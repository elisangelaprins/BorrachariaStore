using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorrachariaStore.Web.Pages;

public class CarrinhoModel : PageModel
{
    private readonly CarrinhoService _carrinhoService;

    public CarrinhoModel(CarrinhoService carrinhoService)
    {
        _carrinhoService = carrinhoService;
    }

    public IReadOnlyList<ItemCarrinho> Itens => _carrinhoService.Itens;

    public decimal Total
    {
        get
        {
          return _carrinhoService.CalcularTotal();
        }
    }

    public void OnGet() { }

    public IActionResult OnPostAtualizarQuantidade(string produtoId, int quantidade)
    {
        if (quantidade < 1)
        {
            _carrinhoService.Remover(produtoId);
        }
        else
        {
            _carrinhoService.AtualizarQuantidade(produtoId, quantidade);
        }
        return RedirectToPage();
    }

    public IActionResult OnPostRemover(string produtoId)
    {
        _carrinhoService.Remover(produtoId);
        return RedirectToPage();
    }
}
