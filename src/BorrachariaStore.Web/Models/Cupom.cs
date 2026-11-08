using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BorrachariaStore.Web.Models;

[BsonIgnoreExtraElements]
public class Cupom
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("codigo")]
    public string Codigo { get; set; } = string.Empty;

    [BsonElement("porcentagemDesconto")]
    public int PorcentagemDesconto { get; set; }

    [BsonElement("valorMinimoPedido")]
    public decimal? ValorMinimoPedido { get; set; }

    [BsonElement("ativo")]
    public bool Ativo { get; set; } = true;

    [BsonElement("validade")]
    public DateTime? Validade { get; set; }

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
