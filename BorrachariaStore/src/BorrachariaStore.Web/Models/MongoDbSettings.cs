namespace BorrachariaStore.Web.Models;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ProdutosCollectionName { get; set; } = string.Empty;
    public string PedidosCollectionName { get; set; } = string.Empty;
}
