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
            // TODO: Retornar o cálculo do total do carrinho via serviço
            throw new NotImplementedException();
        }
    }

    public void OnGet() { }

    public IActionResult OnPostAtualizarQuantidade(string produtoId, int quantidade)
    {
        // TODO: Atualizar quantidade via serviço e redirecionar para a página
        throw new NotImplementedException();
    }

    public IActionResult OnPostRemover(string produtoId)
    {
        // TODO: Remover item via serviço e redirecionar para a página
        throw new NotImplementedException();
    }
}
