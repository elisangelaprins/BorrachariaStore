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

    [BsonElement("cpf")]
    public string? Cpf { get; set; }

    [BsonElement("telefone")]
    public string? Telefone { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("endereco")]
    public string Endereco { get; set; } = string.Empty;

    [BsonElement("cep")]
    public string? Cep { get; set; }

    [BsonElement("metodoPagamento")]
    [BsonRepresentation(BsonType.String)]
    public MetodoPagamento MetodoPagamento { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;

    [BsonElement("itens")]
    public List<ItemCarrinho> Itens { get; set; } = new();

    [BsonElement("subtotal")]
    public decimal Subtotal { get; set; }

    [BsonElement("frete")]
    public decimal Frete { get; set; }

    [BsonElement("desconto")]
    public decimal Desconto { get; set; }

    [BsonElement("codigoCupom")]
    public string? CodigoCupom { get; set; }

    [BsonElement("valorTotal")]
    public decimal ValorTotal { get; set; }

    [BsonElement("dataPedido")]
    public DateTime DataPedido { get; set; } = DateTime.UtcNow;
}
