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
        if (string.IsNullOrWhiteSpace(produtoId))
        {
            return RedirectToPage();
        }

        try
        {
            if (quantidade < 1)
            {
                _carrinhoService.Remover(produtoId);
            }
            else
            {
                _carrinhoService.AtualizarQuantidade(produtoId, quantidade);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERRO] Carrinho OnPostAtualizarQuantidade: {ex.Message}");
        }

        return RedirectToPage();
    }

    public IActionResult OnPostRemover(string produtoId)
    {
        if (string.IsNullOrWhiteSpace(produtoId))
        {
            return RedirectToPage();
        }

        try
        {
            _carrinhoService.Remover(produtoId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERRO] Carrinho OnPostRemover: {ex.Message}");
        }

        return RedirectToPage();
    }
}
