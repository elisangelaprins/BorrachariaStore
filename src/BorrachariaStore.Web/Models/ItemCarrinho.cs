namespace BorrachariaStore.Web.Models;

public class ItemCarrinho
{
    public string ProdutoId { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Foto { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Quantidade { get; set; }

    public int Estoque { get; set; }
    public int QuantidadeMaxima => Estoque > 0 ? Math.Min(8, Estoque) : 8;  

    public decimal Subtotal
    {
        get
        {
            return PrecoUnitario * Quantidade;
        }
    }
}
