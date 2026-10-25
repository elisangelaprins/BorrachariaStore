using System.Text.Json;
using BorrachariaStore.Web.Models;

namespace BorrachariaStore.Web.Services;

public class CarrinhoService
{
    private const string ChaveItens = "Carrinho.Itens";
    private const string ChaveCupom = "Carrinho.Cupom";

    private readonly ISession _session;
    private readonly List<ItemCarrinho> _itens;

    public CarrinhoService(IHttpContextAccessor httpContextAccessor)
    {
        _session = httpContextAccessor.HttpContext!.Session;
        _itens = Ler<List<ItemCarrinho>>(ChaveItens) ?? new();
        CupomAplicado = Ler<Cupom>(ChaveCupom);
    }

    public IReadOnlyList<ItemCarrinho> Itens => _itens;

    public decimal ValorFretePadrao => 100m;
    public decimal ValorMinimoFreteGratis => 800m;

    public void Adicionar(Produto produto, int quantidade = 1)
    {
        if (produto == null || quantidade <= 0 || produto.Estoque <= 0)
        {
            return;
        }
        var ItemCarrinho = _itens.FirstOrDefault(item => item.ProdutoId == produto.Id);

        if (ItemCarrinho != null)
        {
            ItemCarrinho.Estoque = produto.Estoque;
            ItemCarrinho.Quantidade = Math.Min(ItemCarrinho.Quantidade + quantidade, Math.Min(8, produto.Estoque));
        }
        else
        {
            ItemCarrinho novoItem = new ItemCarrinho();
            novoItem.ProdutoId = produto.Id!;
            novoItem.Nome = produto.Nome;
            novoItem.Foto = produto.Fotos?.FirstOrDefault() ?? string.Empty;
            novoItem.PrecoUnitario = produto.Preco;
            novoItem.Estoque = produto.Estoque;
            novoItem.Quantidade = Math.Min(quantidade, Math.Min(8, produto.Estoque));

            _itens.Add(novoItem);
        }

        Salvar();
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
            ItemCarrinho.Quantidade = Math.Min(quantidade, ItemCarrinho.QuantidadeMaxima);
        }

        Salvar();
    }

    public void Remover(string produtoId)
    {
        var ItemCarrinho = _itens.FirstOrDefault(item => item.ProdutoId == produtoId);

        if (ItemCarrinho != null)
        {
            _itens.Remove(ItemCarrinho);
            Salvar();
        }
    }

    public Cupom? CupomAplicado { get; private set; }

    public void AplicarCupom(Cupom cupom)
    {
        CupomAplicado = cupom;
        Salvar();
    }

    public void RemoverCupom()
    {
        CupomAplicado = null;
        Salvar();
    }

    public decimal CalcularDesconto()
    {
        if (CupomAplicado == null || _itens.Count == 0) return 0m;
        var subtotal = CalcularSubtotal();
        if (CupomAplicado.ValorMinimoPedido.HasValue && subtotal < CupomAplicado.ValorMinimoPedido.Value) return 0m;
        return Math.Round(subtotal * (CupomAplicado.PorcentagemDesconto / 100m), 2);
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
        var subtotal = CalcularSubtotal();
        var frete = CalcularFrete();
        var desconto = CalcularDesconto();
        var total = subtotal - desconto + frete;
        return total > 0m ? total : 0m;
    }

    public void Limpar()
    {
        CupomAplicado = null;
        _itens.Clear();
        Salvar();
    }

    private void Salvar()
    {
        _session.SetString(ChaveItens, JsonSerializer.Serialize(_itens));

        if (CupomAplicado == null)
        {
            _session.Remove(ChaveCupom);
        }
        else
        {
            _session.SetString(ChaveCupom, JsonSerializer.Serialize(CupomAplicado));
        }
    }

    private T? Ler<T>(string chave)
    {
        var json = _session.GetString(chave);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }
}
