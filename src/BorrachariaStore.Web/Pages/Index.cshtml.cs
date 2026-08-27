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
        // TODO: Buscar lista de produtos com base nos filtros (Termo e Categoria)
        throw new NotImplementedException();
    }

    public async Task<IActionResult> OnPostAdicionarAoCarrinhoAsync(string produtoId)
    {
        // TODO: Obter o produto por Id e adicionar ao carrinho, depois redirecionar
        throw new NotImplementedException();
    }
}
