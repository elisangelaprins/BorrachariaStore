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

    public decimal Total => _carrinhoService.CalcularTotal();
    public bool PedidoConfirmado { get; set; }

    public void OnGet() { }

    // Aula 5: validação de campos e criação do pedido (checkout simulado)
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var pedido = new Pedido
        {
            NomeComprador = NomeComprador,
            Endereco = Endereco,
            MetodoPagamento = MetodoPagamento,
            Itens = _carrinhoService.Itens.ToList(),
            ValorTotal = Total
        };

        await _pedidoService.CriarAsync(pedido);
        _carrinhoService.Limpar();

        PedidoConfirmado = true;
        return Page();
    }
}
