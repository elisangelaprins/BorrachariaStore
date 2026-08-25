using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages (Aula 2)
builder.Services.AddRazorPages();

// Configuração do MongoDB (Aula 3)
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Serviços de domínio
builder.Services.AddSingleton<ProdutoService>();
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddSingleton<PedidoService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
