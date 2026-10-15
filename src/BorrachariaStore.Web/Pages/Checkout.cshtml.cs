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
    public MetodoPagamento? MetodoPagamento { get; set; }

    [BindProperty] public string? Cpf { get; set; }
    [BindProperty] public string? Telefone { get; set; }
    [BindProperty] public string? Email { get; set; }
    [BindProperty] public string? Cidade { get; set; }
    [BindProperty] public string? Estado { get; set; }
    [BindProperty] public string? Numero { get; set; }
    [BindProperty] public string? Complemento { get; set; }

    public IReadOnlyList<ItemCarrinho> Itens => _carrinhoService.Itens;
    public List<ItemCarrinho> ItensConfirmados { get; set; } = new();
    public decimal TotalConfirmado { get; set; }
    public string? NumeroPedido { get; set; }

    public string? MensagemErroCep { get; set; }

    public decimal Subtotal => _carrinhoService.CalcularSubtotal();

    public decimal Frete => _carrinhoService.CalcularFrete();

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
        ModelState.Clear();

        if (addressData != null)
        {
            Endereco = $"{addressData.Logradouro}, {addressData.Bairro}";
            Cidade = addressData.Localidade;
            Estado = addressData.Uf;
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

        if (_carrinhoService.Itens.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Seu carrinho está vazio.");
            return Page();
        }

        var itensPedido = _carrinhoService.Itens.ToList();
        var valorTotalPedido = _carrinhoService.CalcularTotal();

        var pedido = new Pedido
        {
            NomeComprador = NomeComprador,
            Cpf = Cpf,
            Telefone = Telefone,
            Email = Email,
            Endereco = string.Join(", ", new[] { Endereco, Numero, Complemento, Cidade, Estado }
                .Where(parte => !string.IsNullOrWhiteSpace(parte))),
            MetodoPagamento = MetodoPagamento!.Value,
            Itens = itensPedido,
            ValorTotal = valorTotalPedido,
            Status = StatusPedido.Pendente,
            DataPedido = DateTime.UtcNow
        };

        try
        {
            await _pedidoService.CriarAsync(pedido);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível finalizar o pedido. Tente novamente.");
            return Page();
        }

        ItensConfirmados = itensPedido;
        TotalConfirmado = valorTotalPedido;
        _carrinhoService.Limpar();
        NumeroPedido = pedido.Id;
        PedidoConfirmado = true;

        return Page();
    }
}
