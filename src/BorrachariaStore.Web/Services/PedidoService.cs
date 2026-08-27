using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

// Persistência dos pedidos gerados no Checkout simulado
public class PedidoService
{
    private readonly IMongoCollection<Pedido> _pedidos;

    public PedidoService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _pedidos = database.GetCollection<Pedido>(settings.Value.PedidosCollectionName);
    }

    public async Task CriarAsync(Pedido pedido)
    {
        // TODO: Implementar gravação do pedido no MongoDB
        throw new NotImplementedException();
    }

    public async Task<List<Pedido>> ListarAsync()
    {
        // TODO: Implementar listagem de pedidos gravados
        throw new NotImplementedException();
    }
}