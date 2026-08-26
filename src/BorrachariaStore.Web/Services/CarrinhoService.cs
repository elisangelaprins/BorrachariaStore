using BorrachariaStore.Web.Models;

namespace BorrachariaStore.Web.Services;

// Regras de negócio do Carrinho de Compras (Aula 4) e cálculo do total (Aula 5)
// Escopo: sessão em memória. Persistência real fica fora do escopo do projeto.
public class CarrinhoService
{
    private readonly List<ItemCarrinho> _itens = new();

    public IReadOnlyList<ItemCarrinho> Itens => _itens;

    public void Adicionar(Produto produto, int quantidade = 1)
    {
        var existente = _itens.FirstOrDefault(i => i.ProdutoId == produto.Id);
        if (existente is not null)
        {
            existente.Quantidade += quantidade;
            return;
        }

        _itens.Add(new ItemCarrinho
        {
            ProdutoId = produto.Id ?? string.Empty,
            Nome = produto.Nome,
            UrlFoto = produto.UrlFoto,
            PrecoUnitario = produto.Preco,
            Quantidade = quantidade
        });
    }

    public void AtualizarQuantidade(string produtoId, int quantidade)
    {
        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item is null) return;

        if (quantidade <= 0)
            _itens.Remove(item);
        else
            item.Quantidade = quantidade;
    }

    public void Remover(string produtoId) =>
        _itens.RemoveAll(i => i.ProdutoId == produtoId);

    public decimal CalcularTotal() =>
        _itens.Sum(i => i.Subtotal);

    public void Limpar() => _itens.Clear();
}
