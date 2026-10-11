using BorrachariaStore.Web.Models;

namespace BorrachariaStore.Web.Services;

public class CarrinhoService
{
    private readonly List<ItemCarrinho> _itens = new();

    public IReadOnlyList<ItemCarrinho> Itens => _itens;

    public decimal ValorFretePadrao => 100m;
    public decimal ValorMinimoFreteGratis => 800m;

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
        var ItemCarrinho = _itens.FirstOrDefault(item => item.ProdutoId == produtoId);
      
        if (ItemCarrinho == null)
        {
            return;
        }

        if (quantidade <= 0)
        {
            _itens.Remove(ItemCarrinho);
        }
        else
        {
            ItemCarrinho.Quantidade = quantidade;
        }
    }

    public void Remover(string produtoId)
    {
        var ItemCarrinho = _itens.FirstOrDefault(item => item.ProdutoId == produtoId);

        if (ItemCarrinho != null)
        {
            _itens.Remove(ItemCarrinho);
        }
    }

    public decimal CalcularSubtotal()
    {
        decimal subtotal = 0;
        foreach (ItemCarrinho item in _itens)
        {
            subtotal += item.Subtotal;
        }
        return subtotal;
    }

    public decimal CalcularFrete()
    {
        if (_itens.Count == 0)
        {
            return 0m;
        }

        return CalcularSubtotal() >= ValorMinimoFreteGratis ? 0m : ValorFretePadrao;
    }

    public decimal CalcularTotal()
    {
        return CalcularSubtotal() + CalcularFrete();
    }

    public void Limpar()
    {
        _itens.Clear();
    }
}