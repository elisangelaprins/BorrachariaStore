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
        var produto = await _produtoService.ObterPorIdAsync(id);
        if (produto is null)
            return RedirectToPage("/Admin/Index");

        Produto = produto;
        return Page();
    }

    // Aula 7: rotina de atualização (Update) no MongoDB
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        await _produtoService.AtualizarAsync(Produto.Id!, Produto);
        return RedirectToPage("/Admin/Index");
    }
}
