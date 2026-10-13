using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

// Persistência dos pedidos gerados no Checkout simulado
public class PedidoService
{
    private readonly IMongoCollection<Pedido> _pedidos;
    private readonly ILogger<PedidoService> _logger;

    public PedidoService(IOptions<MongoDbSettings> settings, ILogger<PedidoService> logger)
    {

        _logger = logger;
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.Value.ConnectionString);
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(4);
        var client = new MongoClient(mongoSettings);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _pedidos = database.GetCollection<Pedido>(settings.Value.PedidosCollectionName);
    }

    public async Task CriarAsync(Pedido pedido)
    {
        try
        {
            await _pedidos.InsertOneAsync(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gravar pedido no MongoDB.");
            throw;
        }
    }

    public async Task<List<Pedido>> ListarAsync()
    {
        try
        {
            return await _pedidos.Find(Builders<Pedido>.Filter.Empty).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar pedidos no MongoDB.");
            throw;
        }
    }
}