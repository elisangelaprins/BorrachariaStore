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

    public async Task<List<Produto>> ListarAsync() =>
        await _produtos.Find(_ => true).ToListAsync();

    public async Task<Produto?> ObterPorIdAsync(string id) =>
        await _produtos.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<List<Produto>> BuscarAsync(string? termo, string? categoria) =>
        await _produtos.Find(p =>
                (string.IsNullOrEmpty(termo) || p.Nome.ToLower().Contains(termo.ToLower())) &&
                (string.IsNullOrEmpty(categoria) || p.Categoria == categoria))
            .ToListAsync();

    // Aula 6 - Insert
    public async Task CriarAsync(Produto produto) =>
        await _produtos.InsertOneAsync(produto);

    // Aula 7 - Update
    public async Task AtualizarAsync(string id, Produto produtoAtualizado) =>
        await _produtos.ReplaceOneAsync(p => p.Id == id, produtoAtualizado);

    // Aula 7 - Delete
    public async Task RemoverAsync(string id) =>
        await _produtos.DeleteOneAsync(p => p.Id == id);
}
