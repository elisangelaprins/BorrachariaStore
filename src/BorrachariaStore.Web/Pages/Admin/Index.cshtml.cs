using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorrachariaStore.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ProdutoService _produtoService;

    public IndexModel(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    public List<Produto> Produtos { get; set; } = new();

    public async Task OnGetAsync()
    {
        // TODO: Carregar todos os produtos na lista Produtos via ProdutoService
        throw new NotImplementedException();
    }

    public async Task<IActionResult> OnPostRemoverAsync(string id)
    {
        // TODO: Remover produto por Id via ProdutoService e redirecionar para a página
        throw new NotImplementedException();
    }
}