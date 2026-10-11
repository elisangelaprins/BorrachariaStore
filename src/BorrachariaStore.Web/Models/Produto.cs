using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BorrachariaStore.Web.Models;

[BsonIgnoreExtraElements]
public class Produto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nome")]
    public string Nome { get; set; } = string.Empty;

    [BsonElement("marca")]
    public string Marca { get; set; } = string.Empty;

    [BsonElement("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [BsonElement("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [BsonElement("medida")]
    public string Medida { get; set; } = string.Empty;

    [BsonElement("aro")]
    public int Aro { get; set; }

    [BsonElement("preco")]
    public decimal Preco { get; set; }

    [BsonElement("emPromocao")]
    public bool EmPromocao { get; set; }

    [BsonElement("precoOriginal")]
    public decimal? PrecoOriginal { get; set; }

    [BsonElement("porcentagemDesconto")]
    public int? PorcentagemDesconto { get; set; }

    [BsonElement("avaliacao")]
    public double? Avaliacao { get; set; }

    [BsonElement("urlFoto")]
    public string UrlFoto { get; set; } = string.Empty;

    [BsonElement("fotos")]
    public List<string> Fotos { get; set; } = new();

    [BsonElement("estoque")]
    public int Estoque { get; set; }
}
