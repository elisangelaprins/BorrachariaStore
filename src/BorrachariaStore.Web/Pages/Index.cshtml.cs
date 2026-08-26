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

    // Aula 3: buscar lista de produtos do MongoDB e aplicar filtros
    public async Task OnGetAsync()
    {
        Produtos = await _produtoService.BuscarAsync(Termo, Categoria);
    }

    // Aula 4: adicionar ao carrinho
    public async Task<IActionResult> OnPostAdicionarAoCarrinhoAsync(string produtoId)
    {
        var produto = await _produtoService.ObterPorIdAsync(produtoId);
        if (produto is not null)
            _carrinhoService.Adicionar(produto);

        return RedirectToPage();
    }
}
