using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

// Responsável pelo catálogo (Aula 3) e pelo CRUD do painel admin (Aulas 6 e 7)
public class ProdutoService
{
    private readonly IMongoCollection<Produto> _produtos;

    public ProdutoService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _produtos = database.GetCollection<Produto>(settings.Value.ProdutosCollectionName);
    }

    public async Task<List<Produto>> ListarAsync()
    {
        // TODO: Implementar listagem de todos os produtos
        throw new NotImplementedException();
    }

    public async Task<Produto?> ObterPorIdAsync(string id)
    {
        // TODO: Implementar busca de produto por Id
        throw new NotImplementedException();
    }

    public async Task<List<Produto>> BuscarAsync(string? termo, string? categoria)
    {
        try
        {
            var filtro = Builders<Produto>.Filter.Empty; //filtro que busca todos os registros
            var listaDeProdutos = await _produtos.Find(filtro).ToListAsync(); //execura a busca no Mongo
            return listaDeProdutos;
        } catch (Exception)
        {
            return new List<Produto>(); // em caso de erro no banco
        }

    }

    public async Task CriarAsync(Produto produto)
    {
        // TODO: Implementar inserção de produto
        throw new NotImplementedException();
    }

    public async Task AtualizarAsync(string id, Produto produtoAtualizado)
    {
        // TODO: Implementar atualização (Replace) de produto
        throw new NotImplementedException();
    }

    public async Task RemoverAsync(string id)
    {
        // TODO: Implementar exclusão de produto por Id
        throw new NotImplementedException();
    }
}
