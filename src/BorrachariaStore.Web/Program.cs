using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddHttpClient();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<ProdutoService>();
builder.Services.AddSingleton<CarrinhoService>();
builder.Services.AddSingleton<PedidoService>();
builder.Services.AddSingleton<ViaCepService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

var defaultCulture = new System.Globalization.CultureInfo("pt-BR");
defaultCulture.NumberFormat.NumberDecimalSeparator = ",";
defaultCulture.NumberFormat.CurrencyDecimalSeparator = ",";
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(defaultCulture),
    SupportedCultures = new List<System.Globalization.CultureInfo> { defaultCulture },
    SupportedUICultures = new List<System.Globalization.CultureInfo> { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.MapPost("/api/carrinho/adicionar", async (string produtoId, ProdutoService produtoService, CarrinhoService carrinhoService) =>
{
    if (string.IsNullOrWhiteSpace(produtoId))
    {
        return Results.BadRequest(new { success = false, message = "ID do produto inválido." });
    }

    var produto = await produtoService.ObterPorIdAsync(produtoId);
    if (produto == null)
    {
        return Results.NotFound(new { success = false, message = "Produto não encontrado." });
    }

    carrinhoService.Adicionar(produto, 1);

    return Results.Ok(new
    {
        success = true,
        totalItens = carrinhoService.Itens.Sum(i => i.Quantidade),
        nomeProduto = produto.Nome
    });
});

app.Run();
