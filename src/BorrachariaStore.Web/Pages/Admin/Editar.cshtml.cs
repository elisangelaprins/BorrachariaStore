using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorrachariaStore.Web.Pages.Admin;

public class EditarModel : PageModel
{
    private readonly ProdutoService _produtoService;

    public EditarModel(ProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [BindProperty]
    public Produto Produto { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToPage("/Admin/Index");
        }

        var product = await _produtoService.ObterPorIdAsync(id);

        if (product == null)
        {
            return RedirectToPage("/Admin/Index");
        }

        Produto = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // TODO: Validar ModelState, atualizar Produto via ProdutoService e redirecionar para /Admin/Index
        throw new NotImplementedException();
    }
}
