using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BorrachariaStore.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ProdutoService _produtoService;
    private readonly CarrinhoService _carrinhoService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ProdutoService produtoService, CarrinhoService carrinhoService, ILogger<IndexModel> logger)
    {
        _produtoService = produtoService;
        _carrinhoService = carrinhoService;
        _logger = logger;
    }

    public List<Produto> Produtos { get; set; } = new();
    public string? ErroCarregamento { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Termo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Categoria { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Produtos = await _produtoService.BuscarAsync(Termo, Categoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha de conexão com o MongoDB ao carregar a página inicial.");
            ErroCarregamento = "Nosso catálogo está temporariamente indisponível. Por favor, tente novamente em instantes.";
            Produtos = new List<Produto>();
        }
    }

    public async Task<IActionResult> OnPostAdicionarAoCarrinhoAsync(string produtoId)
    {
        try
        {
            var produto = await _produtoService.ObterPorIdAsync(produtoId);
            if (produto != null)
            {
                _carrinhoService.Adicionar(produto, 1);
                TempData["MensagemSucesso"] = $"\"{produto.Nome}\" foi adicionado ao seu carrinho!";
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers.Accept.ToString().Contains("application/json"))
            {
                return new JsonResult(new
                {
                    success = produto != null,
                    totalItens = _carrinhoService.Itens.Sum(i => i.Quantidade),
                    nomeProduto = produto?.Nome ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao adicionar o produto {ProdutoId} ao carrinho.", produtoId);
            TempData["MensagemErro"] = "Ocorreu uma falha ao comunicar com o banco de dados ao adicionar o produto.";
        }

        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostComprarAgoraAsync(string termo)
    {
        try
        {
            var produtos = await _produtoService.BuscarAsync(termo, null);
            var produto = produtos.FirstOrDefault();
            if (produto != null)
            {
                _carrinhoService.Adicionar(produto, 1);
                TempData["MensagemSucesso"] = $"\"{produto.Nome}\" foi adicionado ao seu carrinho!";
            }
            else
            {
                TempData["MensagemErro"] = "Produto temporariamente indisponível.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao comprar agora o produto {Termo}.", termo);
            TempData["MensagemErro"] = "Não foi possível adicionar o produto ao carrinho no momento. Tente novamente em instantes.";
        }

        return RedirectToPage("/Carrinho");
    }
}