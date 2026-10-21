using System.Text.Json;
using System.Text.RegularExpressions;
using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

public class ProdutoService
{
    private readonly IMongoCollection<Produto> _produtos;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(IOptions<MongoDbSettings> settings, ILogger<ProdutoService> logger, IWebHostEnvironment webHostEnvironment)
    {
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(4);
        var client = new MongoClient(mongoSettings);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _produtos = database.GetCollection<Produto>(settings.Value.ProdutosCollectionName);
        _logger = logger;

        try
        {
            if (_produtos.CountDocuments(Builders<Produto>.Filter.Empty) == 0)
            {
                var jsonPath = Path.Combine(webHostEnvironment.ContentRootPath, "Data", "MockData", "produtos.json");
                if (File.Exists(jsonPath))
                {
                    var json = File.ReadAllText(jsonPath);
                    var lista = JsonSerializer.Deserialize<List<Produto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (lista != null && lista.Count > 0) _produtos.InsertMany(lista);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível realizar o seed inicial de produtos no MongoDB.");
        }
    }

    public async Task<List<Produto>> ListarAsync()
    {
        try
        {
            var filter = Builders<Produto>.Filter.Empty;
            return await _produtos.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos no MongoDB.");
            throw;
        }
    }

    public async Task<Produto?> ObterPorIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        try
        {
            var filter = Builders<Produto>.Filter.Eq(p => p.Id, id);
            return await _produtos.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por ID {ProdutoId} no MongoDB.", id);
            throw;
        }
    }

    public async Task<List<Produto>> BuscarAsync(string? termo, string? categoria)
    {
        try
        {
            var filter = Builders<Produto>.Filter.Empty;

            if (!string.IsNullOrWhiteSpace(termo))
            {
                var normalizedTerm = termo.Trim();
                var searchRegex = new MongoDB.Bson.BsonRegularExpression(Regex.Escape(normalizedTerm), "i");
                var textFilter = Builders<Produto>.Filter.Or(
                    Builders<Produto>.Filter.Regex(p => p.Nome, searchRegex),
                    Builders<Produto>.Filter.Regex(p => p.Marca, searchRegex),
                    Builders<Produto>.Filter.Regex(p => p.Medida, searchRegex)
                );

                if (int.TryParse(normalizedTerm, out int aroNumber))
                {
                    textFilter = Builders<Produto>.Filter.Or(textFilter, Builders<Produto>.Filter.Eq(p => p.Aro, aroNumber));
                }

                filter &= textFilter;
            }

            if (!string.IsNullOrWhiteSpace(categoria))
            {
                var categoryFilter = Builders<Produto>.Filter.Eq(
                    p => p.Categoria,
                    categoria.Trim());

                filter &= categoryFilter;
            }
            
            return await _produtos.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos no MongoDB com termo '{Termo}' e categoria '{Categoria}'.", termo, categoria);
            throw;
        }
    }

    public async Task CriarAsync(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);
        try {
            await _produtos.InsertOneAsync(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inserir o produto {ProdutoId}", produto.Id);
            throw;
        }
    }

    public async Task AtualizarAsync(string id, Produto produtoAtualizado)
    {
        if (string.IsNullOrWhiteSpace(id) || produtoAtualizado == null)
        {
            return;
        }

        try
        {
            produtoAtualizado.Id = id;
            var filter = Builders<Produto>.Filter.Eq(p => p.Id, id);
            await _produtos.ReplaceOneAsync(filter, produtoAtualizado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProdutoId} no MongoDB.", id);
            throw;
        }
    }

    public async Task RemoverAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        try
        {
            var filter = Builders<Produto>.Filter.Eq(p => p.Id, id);
            await _produtos.DeleteOneAsync(filter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover produto {ProdutoId} no MongoDB.", id);
            throw;
        }
    }
}
