using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

// Persistência dos pedidos gerados no Checkout simulado (Aula 5)
public class PedidoService
{
    private readonly IMongoCollection<Pedido> _pedidos;

    public PedidoService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _pedidos = database.GetCollection<Pedido>(settings.Value.PedidosCollectionName);
    }

    public async Task CriarAsync(Pedido pedido) =>
        await _pedidos.InsertOneAsync(pedido);

    public async Task<List<Pedido>> ListarAsync() =>
        await _pedidos.Find(_ => true).ToListAsync();
}
