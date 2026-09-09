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
    public string? MensagemErro { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var productList = await _produtoService.ListarAsync();
            Produtos = productList ?? new List<Produto>();

            if (Produtos.Count == 0)
            {
                ViewData["Aviso"] = "Nenhum produto cadastrado no momento.";
            }
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"[LOG ERRO] Admin Index OnGetAsync: {ex.Message}");
            Produtos = new List<Produto>();
            MensagemErro = "Falha ao carregar a lista de produtos. Tente novamente mais tarde.";
        }
    }


    public async Task<IActionResult> OnPostRemoverAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToPage();
        }

        try 
        {
            await _produtoService.RemoverAsync(id);
            TempData["MensagemSucesso"] = "Produto excluído com sucesso!";
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERRO] Admin Index OnPostRemoverAsync: {ex.Message}");
            TempData["MensagemErro"] = "Ocorreu um erro ao tentar excluir o produto.";
        }

        return RedirectToPage();
    }
}