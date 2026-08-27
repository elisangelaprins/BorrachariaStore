using BorrachariaStore.Web.Models;

namespace BorrachariaStore.Web.Services;

// Regras de negócio do Carrinho de Compras e cálculo do total
public class CarrinhoService
{
    private readonly List<ItemCarrinho> _itens = new();

    public IReadOnlyList<ItemCarrinho> Itens => _itens;

    public void Adicionar(Produto produto, int quantidade = 1)
    {
        // TODO: Implementar adição de produto ao carrinho ou incremento se já existir
        throw new NotImplementedException();
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
        // TODO: Implementar cálculo do total do carrinho (soma dos subtotais)
        throw new NotImplementedException();
    }

    public void Limpar()
    {
        // TODO: Implementar limpeza de todos os itens do carrinho
        throw new NotImplementedException();
    }
}