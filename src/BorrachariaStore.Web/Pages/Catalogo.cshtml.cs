using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BorrachariaStore.Web.Pages;

public class CatalogoModel : PageModel
{
    private readonly ProdutoService _produtoService;
    private readonly CarrinhoService _carrinhoService;
    private readonly ILogger<CatalogoModel> _logger;

    public CatalogoModel(ProdutoService produtoService, CarrinhoService carrinhoService, ILogger<CatalogoModel> logger)
    {
        _produtoService = produtoService;
        _carrinhoService = carrinhoService;
        _logger = logger;
    }

    public List<Produto> Produtos { get; set; } = new();
    public int TotalProdutos => Produtos.Count;
    public string? ErroCarregamento { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Termo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Categoria { get; set; } = "Todos";

    [BindProperty(SupportsGet = true)]
    public List<int> Aros { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public List<string> Marcas { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string Ordenacao { get; set; } = "relevancia";

    [BindProperty(SupportsGet = true)]
    public bool ApenasOfertas { get; set; }

    public readonly List<int> ArosDisponiveis = new() { 13, 14, 15, 16, 17, 18 };
    public readonly List<string> MarcasDisponiveis = new() { "Goodyear", "Pirelli", "Michelin", "Bridgestone", "Continental" };
    public readonly List<string> CategoriasRapidas = new() { "Todos", "Passeio", "SUV", "Carga", "Performance", "Off-Road" };

    public async Task OnGetAsync()
    {
        try
        {
            var todos = await _produtoService.ListarAsync();
            var query = todos.AsEnumerable();

            if (ApenasOfertas)
            {
                query = query.Where(p => p.EmPromocao || (p.PrecoOriginal.HasValue && p.PrecoOriginal.Value > p.Preco && p.PrecoOriginal.Value > 0));
            }

            if (!string.IsNullOrWhiteSpace(Termo))
            {
                var termoNorm = Termo.Trim().ToLowerInvariant();
                query = query.Where(p =>
                    (!string.IsNullOrEmpty(p.Nome) && p.Nome.ToLowerInvariant().Contains(termoNorm)) ||
                    (!string.IsNullOrEmpty(p.Marca) && p.Marca.ToLowerInvariant().Contains(termoNorm)) ||
                    (!string.IsNullOrEmpty(p.Medida) && p.Medida.ToLowerInvariant().Contains(termoNorm)));
            }

            if (!string.IsNullOrWhiteSpace(Categoria) && !Categoria.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p => string.Equals(p.Categoria, Categoria, StringComparison.OrdinalIgnoreCase));
            }

            if (Aros != null && Aros.Any())
            {
                query = query.Where(p => Aros.Contains(p.Aro));
            }

            if (Marcas != null && Marcas.Any())
            {
                query = query.Where(p => Marcas.Any(m => string.Equals(m, p.Marca, StringComparison.OrdinalIgnoreCase)));
            }

            query = Ordenacao switch
            {
                "menor-preco" => query.OrderBy(p => p.Preco),
                "maior-preco" => query.OrderByDescending(p => p.Preco),
                "avaliacao" => query.OrderByDescending(p => p.Avaliacao ?? 4.8),
                "relevancia" or _ => query.OrderByDescending(p => p.EmPromocao).ThenByDescending(p => p.Avaliacao ?? 4.8)
            };

            Produtos = query.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar catálogo de produtos do MongoDB.");
            ErroCarregamento = "Catálogo temporariamente indisponível. Tente novamente mais tarde.";
            Produtos = new List<Produto>();
        }
    }

    public async Task<IActionResult> OnPostAdicionarAoCarrinhoAsync(string produtoId)
    {
        if (string.IsNullOrWhiteSpace(produtoId))
        {
            TempData["MensagemErro"] = "Não foi possível identificar o produto. Por favor, tente novamente.";
            return RedirectToPage("/Catalogo");
        }

        try
        {
            var produto = await _produtoService.ObterPorIdAsync(produtoId);
            if (produto != null)
            {
                _carrinhoService.Adicionar(produto, 1);
                TempData["MensagemSucesso"] = $"\"{produto.Nome}\" foi adicionado ao seu carrinho!";
            }
            else
            {
                TempData["MensagemErro"] = "O produto selecionado não está mais disponível.";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return new JsonResult(new
                {
                    success = produto != null,
                    totalItens = _carrinhoService.Itens.Sum(i => i.Quantidade),
                    nomeProduto = produto?.Nome ?? string.Empty,
                    mensagem = produto != null ? null : "Produto não encontrado."
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar produto {ProdutoId} ao carrinho.", produtoId);
            TempData["MensagemErro"] = "Não foi possível adicionar o produto ao carrinho no momento.";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return new JsonResult(new { success = false, totalItens = _carrinhoService.Itens.Sum(i => i.Quantidade), mensagem = "Erro de conexão com o banco de dados." });
            }
        }

        var referer = Request.Headers.Referer.ToString();
        if (!string.IsNullOrWhiteSpace(referer) && Url.IsLocalUrl(referer))
        {
            return Redirect(referer);
        }

        return RedirectToPage("/Catalogo");
    }
}
