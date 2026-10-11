using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorrachariaStore.Web.Pages.Admin;

public class CadastrarModel : PageModel
{
    private readonly ProdutoService _produtoService;

    public CadastrarModel(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [BindProperty]
    public Produto Produto { get; set; } = new();

    [BindProperty]
    public decimal PrecoNormal { get; set; }

    [BindProperty]
    public decimal? PrecoPromocional { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: Salvar o produto no MongoDB via ProdutoService:
        // await _produtoService.CriarAsync(Produto);
        // return RedirectToPage("/Admin/Index");

        return Page();
    }
}
