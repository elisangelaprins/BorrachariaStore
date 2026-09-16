using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BorrachariaStore.Web.Services;

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
        try {
            var filter = Builders<Produto>.Filter.Empty;
            var products = await _produtos.Find(filter).ToListAsync();
            return products;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERRO] ListarAsync: {ex.Message}");
            return new List<Produto>();
        }
    }

    public async Task<Produto?> ObterPorIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        try {
            var filter = Builders<Produto>.Filter.Eq(p => p.Id, id);
            var product = await _produtos.Find(filter).FirstOrDefaultAsync();

            return product;   
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"[LOG ERRO] ObterPorIdAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Produto>> BuscarAsync(string? termo, string? categoria)
    {
        try
        {
            var filter = Builders<Produto>.Filter.Empty; //filtro que busca todos os registros
            var products = await _produtos.Find(filter).ToListAsync(); //execura a busca no Mongo
            return products;
        } catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERRO] BuscarAsync: {ex.Message}");
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
        if (string.IsNullOrWhiteSpace(id) || produtoAtualizado == null) {
            return;
        }

        try {
            produtoAtualizado.Id = id;

            var filter = Builders<Produto>.Filter.Eq(p => p.Id, id);
            var result = await _produtos.ReplaceOneAsync(filter, produtoAtualizado);
        }
        catch (Exception ex) {
            Console.WriteLine($"[LOG ERRO] AtualizarAsync: {ex.Message}");
        }
    }

    public async Task RemoverAsync(string id)
    {
       if (string.IsNullOrWhiteSpace(id))
       {
        return;
       }

       try {
        var filter = Builders<Produto>.Filter.Eq(p => p.Id, id);
        var result = await _produtos.DeleteOneAsync(filter);

       }
       catch (Exception ex)
       {
        Console.WriteLine($"[LOG ERRO] RemoverAsync: {ex.Message}");
       }
    }
}
