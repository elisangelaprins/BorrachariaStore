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

    // Aula 5: cálculo do valor total
    public decimal Total => _carrinhoService.CalcularTotal();

    public void OnGet() { }

    // Aula 4: controle de aumento/redução de quantidade
    public IActionResult OnPostAtualizarQuantidade(string produtoId, int quantidade)
    {
        _carrinhoService.AtualizarQuantidade(produtoId, quantidade);
        return RedirectToPage();
    }

    // Aula 4: botão de exclusão de item
    public IActionResult OnPostRemover(string produtoId)
    {
        _carrinhoService.Remover(produtoId);
        return RedirectToPage();
    }
}
