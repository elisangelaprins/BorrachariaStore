using BorrachariaStore.Web.Models;

namespace BorrachariaStore.Web.Services;

// Regras de negócio do Carrinho de Compras e cálculo do total
public class CarrinhoService
{
    private readonly List<ItemCarrinho> _itens = new();

    public IReadOnlyList<ItemCarrinho> Itens => _itens;

    public void Adicionar(Produto produto, int quantidade = 1)
    {
        if (produto == null || quantidade <= 0)
        {
            return;
        }
        var ItemCarrinho = _itens.FirstOrDefault(item => item.ProdutoId == produto.Id);

        if (ItemCarrinho != null)
        {
            ItemCarrinho.Quantidade = ItemCarrinho.Quantidade + quantidade;
        }
        else
        {
            ItemCarrinho novoItem = new ItemCarrinho();
            novoItem.ProdutoId = produto.Id;
            novoItem.Nome = produto.Nome;
            novoItem.UrlFoto = produto.UrlFoto;
            novoItem.PrecoUnitario = produto.Preco;
            novoItem.Quantidade = quantidade;

            _itens.Add(novoItem);
        }
    }

    public void AtualizarQuantidade(string produtoId, int quantidade)
    {
        // TODO: Implementar atualização da quantidade ou remoção se quantidade <= 0
        throw new NotImplementedException();
    }

    public void Remover(string produtoId)
    {
        // TODO: Implementar remoção do item do carrinho
        throw new NotImplementedException();
    }

    public decimal CalcularTotal()
    {
        decimal valorTotal = 0;

        foreach (ItemCarrinho item in _itens)
        {
            valorTotal = valorTotal + item.Subtotal;
        }

        return valorTotal;
    }

    public void Limpar()
    {
        // TODO: Implementar limpeza de todos os itens do carrinho
        throw new NotImplementedException();
    }
}