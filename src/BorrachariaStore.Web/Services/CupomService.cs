using System.Text.Json;
using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

public class ValidacaoCupomResultado
{
    public bool Valido { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public Cupom? Cupom { get; set; }
    public decimal ValorDesconto { get; set; }
}

public class CupomService
{
    private readonly IMongoCollection<Cupom> _cupons;
    private readonly ILogger<CupomService> _logger;

    public CupomService(IOptions<MongoDbSettings> settings, ILogger<CupomService> logger, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(4);
        var client = new MongoClient(mongoSettings);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _cupons = database.GetCollection<Cupom>("Cupons");

        try
        {
            if (_cupons.CountDocuments(Builders<Cupom>.Filter.Empty) == 0)
            {
                var jsonPath = Path.Combine(webHostEnvironment.ContentRootPath, "Data", "MockData", "cupons.json");
                if (File.Exists(jsonPath))
                {
                    var json = File.ReadAllText(jsonPath);
                    var lista = JsonSerializer.Deserialize<List<Cupom>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (lista != null && lista.Count > 0) _cupons.InsertMany(lista);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível realizar o seed inicial de cupons no MongoDB.");
        }
    }

    public async Task<List<Cupom>> ListarTodosAsync()
    {
        try
        {
            return await _cupons.Find(Builders<Cupom>.Filter.Empty).SortByDescending(c => c.CriadoEm).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar cupons no MongoDB.");
            return new List<Cupom>();
        }
    }

    public async Task<Cupom?> ObterPorCodigoAsync(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo)) return null;

        try
        {
            var codigoNorm = codigo.Trim().ToUpperInvariant();
            return await _cupons.Find(c => c.Codigo == codigoNorm).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cupom {Codigo} no MongoDB.", codigo);
            return null;
        }
    }

    public async Task CriarAsync(Cupom cupom)
    {
        cupom.Codigo = cupom.Codigo.Trim().ToUpperInvariant();
        cupom.CriadoEm = DateTime.UtcNow;
        await _cupons.InsertOneAsync(cupom);
    }

    public async Task AlternarStatusAsync(string id, bool ativo)
    {
        var filter = Builders<Cupom>.Filter.Eq(c => c.Id, id);
        var update = Builders<Cupom>.Update.Set(c => c.Ativo, ativo);
        await _cupons.UpdateOneAsync(filter, update);
    }

    public async Task ExcluirAsync(string id)
    {
        var filter = Builders<Cupom>.Filter.Eq(c => c.Id, id);
        await _cupons.DeleteOneAsync(filter);
    }

    public async Task AtualizarAsync(Cupom cupom)
    {
        var filter = Builders<Cupom>.Filter.Eq(c => c.Id, cupom.Id);
        var update = Builders<Cupom>.Update
            .Set(c => c.Codigo, cupom.Codigo.Trim().ToUpperInvariant())
            .Set(c => c.PorcentagemDesconto, cupom.PorcentagemDesconto)
            .Set(c => c.ValorMinimoPedido, cupom.ValorMinimoPedido)
            .Set(c => c.Validade, cupom.Validade);
        await _cupons.UpdateOneAsync(filter, update);
    }

    public async Task<ValidacaoCupomResultado> ValidarCupomAsync(string codigo, decimal subtotal)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return new ValidacaoCupomResultado { Valido = false, Mensagem = "Informe um código de cupom." };
        }

        var cupom = await ObterPorCodigoAsync(codigo);
        if (cupom == null)
        {
            return new ValidacaoCupomResultado { Valido = false, Mensagem = "Cupom não encontrado." };
        }

        if (!cupom.Ativo)
        {
            return new ValidacaoCupomResultado { Valido = false, Mensagem = "Este cupom não está mais disponível." };
        }

        if (cupom.Validade.HasValue && cupom.Validade.Value < DateTime.UtcNow)
        {
            return new ValidacaoCupomResultado { Valido = false, Mensagem = "Este cupom expirou." };
        }

        if (cupom.ValorMinimoPedido.HasValue && subtotal < cupom.ValorMinimoPedido.Value)
        {
            return new ValidacaoCupomResultado
            {
                Valido = false,
                Mensagem = $"Valor mínimo de R$ {cupom.ValorMinimoPedido.Value:N2} para usar este cupom."
            };
        }

        var valorDesconto = Math.Round(subtotal * (cupom.PorcentagemDesconto / 100m), 2);

        return new ValidacaoCupomResultado
        {
            Valido = true,
            Cupom = cupom,
            ValorDesconto = valorDesconto,
            Mensagem = $"Cupom {cupom.Codigo} aplicado! {cupom.PorcentagemDesconto}% de desconto."
        };
    }
}
