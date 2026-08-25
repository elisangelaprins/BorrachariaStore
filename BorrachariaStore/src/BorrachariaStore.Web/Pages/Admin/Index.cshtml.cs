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
        Produtos = await _produtoService.ListarAsync();
    }

    // Aula 7: rotina de remoção (Delete) no MongoDB
    public async Task<IActionResult> OnPostRemoverAsync(string id)
    {
        await _produtoService.RemoverAsync(id);
        return RedirectToPage();
    }
}
