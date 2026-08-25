using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BorrachariaStore.Web.Models;

// Pedido gerado no Checkout simulado (Aula 5)
public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nomeComprador")]
    public string NomeComprador { get; set; } = string.Empty;

    [BsonElement("endereco")]
    public string Endereco { get; set; } = string.Empty;

    [BsonElement("metodoPagamento")]
    public string MetodoPagamento { get; set; } = string.Empty; // Cartão, Pix, Boleto

    [BsonElement("itens")]
    public List<ItemCarrinho> Itens { get; set; } = new();

    [BsonElement("valorTotal")]
    public decimal ValorTotal { get; set; }

    [BsonElement("dataPedido")]
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;
}
