using System.Text.Json.Serialization;
using ThirdParty.Json.LitJson;

namespace BorrachariaStore.Web.Models;

public class EnderecaoCepDto
{
    [JsonPropertyName("cep")]
    public string? Cep { get; set; }

    [JsonPropertyName("logradouro")]
    public string? Logradouro { get; set; }

    [JsonPropertyName("completo")]
    public string? Completo { get; set; }

    [JsonPropertyName("bairro")]
    public string? Bairro { get; set; }

    [JsonPropertyName("localidade")]
    public string? Localidade { get; set; }

    [JsonPropertyName("uf")]
    public string? Uf { get; set;}

    [JsonPropertyName("erro")]
    public string? Erro { get; set; }

}