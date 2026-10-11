using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

public class ProdutoService
{
    private readonly IMongoCollection<Produto> _produtos;
    private readonly ILogger<ProdutoService> _logger;

    public ProdutoService(IOptions<MongoDbSettings> settings, ILogger<ProdutoService> logger)
    {
        _logger = logger;
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(4);
        var client = new MongoClient(mongoSettings);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _produtos = database.GetCollection<Produto>(settings.Value.ProdutosCollectionName);
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
                var searchRegex = new MongoDB.Bson.BsonRegularExpression(termo, "i");
                var textFilter = Builders<Produto>.Filter.Or(
                    Builders<Produto>.Filter.Regex(p => p.Nome, searchRegex),
                    Builders<Produto>.Filter.Regex(p => p.Marca, searchRegex),
                    Builders<Produto>.Filter.Regex(p => p.Medida, searchRegex)
                );

                if (int.TryParse(termo, out int aroNumber))
                {
                    textFilter = Builders<Produto>.Filter.Or(textFilter, Builders<Produto>.Filter.Eq(p => p.Aro, aroNumber));
                }

                filter &= textFilter;
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
        throw new NotImplementedException();
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
