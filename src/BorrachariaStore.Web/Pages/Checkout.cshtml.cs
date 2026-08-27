using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BorrachariaStore.Web.Pages;

public class CheckoutModel : PageModel
{
    private readonly CarrinhoService _carrinhoService;
    private readonly PedidoService _pedidoService;

    public CheckoutModel(CarrinhoService carrinhoService, PedidoService pedidoService)
    {
        _carrinhoService = carrinhoService;
        _pedidoService = pedidoService;
    }

    [BindProperty]
    [Required]
    public string NomeComprador { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string Endereco { get; set; } = string.Empty;

    [BindProperty]
    [Required]
    public string MetodoPagamento { get; set; } = string.Empty;

    public decimal Total
    {
        get
        {
            // TODO: Retornar o cálculo do total do carrinho via serviço
            throw new NotImplementedException();
        }
    }

    public bool PedidoConfirmado { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        // TODO: Validar ModelState, montar o Pedido, salvar via PedidoService, limpar o carrinho e setar PedidoConfirmado
        throw new NotImplementedException();
    }
}
