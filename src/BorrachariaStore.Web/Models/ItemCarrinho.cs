namespace BorrachariaStore.Web.Models;

// Item dentro do Carrinho de Compras (Aula 4)
public class ItemCarrinho
{
    public string ProdutoId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string UrlFoto { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }

    public decimal Subtotal => PrecoUnitario * Quantidade;
}
