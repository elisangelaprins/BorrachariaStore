using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorrachariaStore.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ProdutoService _produtoService;
    private readonly CarrinhoService _carrinhoService;

    public IndexModel(ProdutoService produtoService, CarrinhoService carrinhoService)
    {
        _produtoService = produtoService;
        _carrinhoService = carrinhoService;
    }

    public List<Produto> Produtos { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Termo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Categoria { get; set; }

    public async Task OnGetAsync()
    {
        Produtos = await _produtoService.BuscarAsync(Termo, Categoria);
    }

    public async Task<IActionResult> OnPostAdicionarAoCarrinhoAsync(string produtoId)
    {
        var produto = await _produtoService.ObterPorIdAsync(produtoId);
        if (produto != null)
        {
            _carrinhoService.Adicionar(produto, 1);
        }
        return RedirectToPage("/Carrinho");
    }
}