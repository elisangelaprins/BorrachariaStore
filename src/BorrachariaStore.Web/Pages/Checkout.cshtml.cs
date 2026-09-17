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

    private readonly ViaCepService _viaCepService;

    public CheckoutModel(CarrinhoService carrinhoService, PedidoService pedidoService, ViaCepService viaCepService)
    {
        _carrinhoService = carrinhoService;
        _pedidoService = pedidoService;
        _viaCepService = viaCepService;
    }

    [BindProperty]
    [Required(ErrorMessage = "O nome do comprador é obrigatório")]
    public string NomeComprador { get; set; } = string.Empty;

    [BindProperty]
    public string? Cep { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "O endereço é obrigatório")]
    public string Endereco { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Selecione a forma de pagamento")]
    public string MetodoPagamento { get; set; } = string.Empty;

    public string? MensagemErroCep { get; set; }

    public decimal Total => _carrinhoService.CalcularTotal();

    public bool PedidoConfirmado { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostBuscarCepAsync()
    {
        if (string.IsNullOrWhiteSpace(Cep))
        {
            MensagemErroCep = "Por favor, digite um CEP válido.";
            return Page();
        }

        var addressData = await _viaCepService.BuscarEnderecoPorCepAsync(Cep);

        if (addressData != null)
        {
            Endereco = $"{addressData.Logradouro}, {addressData.Bairro}, {addressData.Localidade} - {addressData.Uf}";
            MensagemErroCep = null;

        }
        else
        {
            MensagemErroCep = "CEP não encontrado ou inválido.";
        }

        return Page();
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return Page();
    }
}
