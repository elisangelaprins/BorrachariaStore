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
    private readonly ProdutoService _produtoService;
    private readonly ViaCepService _viaCepService;
    private readonly ILogger<CheckoutModel> _logger;

    public CheckoutModel(
        CarrinhoService carrinhoService,
        PedidoService pedidoService,
        ProdutoService produtoService,
        ViaCepService viaCepService,
        ILogger<CheckoutModel> logger)
    {
        _carrinhoService = carrinhoService;
        _pedidoService = pedidoService;
        _produtoService = produtoService;
        _viaCepService = viaCepService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "O nome do comprador é obrigatório")]
    public string NomeComprador { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "O CEP é obrigatório")]
    [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido. Use o formato 00000-000")]
    public string? Cep { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "O endereço é obrigatório")]
    public string Endereco { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Selecione a forma de pagamento")]
    public MetodoPagamento? MetodoPagamento { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "O CPF é obrigatório")]
    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$", ErrorMessage = "CPF inválido. Use o formato 000.000.000-00")]
    public string? Cpf { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "O telefone é obrigatório")]
    [RegularExpression(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$", ErrorMessage = "Telefone inválido. Use o formato (00) 00000-0000")]
    public string? Telefone { get; set; }

    [BindProperty] public string? Email { get; set; }
    [BindProperty] public string? Cidade { get; set; }
    [BindProperty] public string? Estado { get; set; }
    [BindProperty] public string? Numero { get; set; }
    [BindProperty] public string? Complemento { get; set; }

    public IReadOnlyList<ItemCarrinho> Itens => _carrinhoService.Itens;

    public string? MensagemErroCep { get; set; }

    public decimal Subtotal => _carrinhoService.CalcularSubtotal();

    public decimal Frete => _carrinhoService.CalcularFrete();

    public decimal Desconto => _carrinhoService.CalcularDesconto();

    public Cupom? CupomAplicado => _carrinhoService.CupomAplicado;

    public decimal Total => _carrinhoService.CalcularTotal();

    public Pedido? PedidoRealizado { get; set; }

    public bool PedidoConfirmado => PedidoRealizado != null;

    public async Task<IActionResult> OnGetAsync(string? pedidoId)
    {
        if (string.IsNullOrWhiteSpace(pedidoId))
        {
            return Page();
        }

        try
        {
            PedidoRealizado = await _pedidoService.ObterPorIdAsync(pedidoId);
        }
        catch (Exception)
        {
            PedidoRealizado = null;
        }

        if (PedidoRealizado == null)
        {
            return RedirectToPage();
        }

        return Page();
    }

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
        var desconto = _carrinhoService.CalcularDesconto();

        var pedido = new Pedido
        {
            NomeComprador = NomeComprador,
            Cpf = Cpf,
            Telefone = Telefone,
            Email = Email,
            Endereco = string.Join(", ", new[] { Endereco, Numero, Complemento, Cidade, Estado }
                .Where(parte => !string.IsNullOrWhiteSpace(parte))),
            Cep = Cep,
            MetodoPagamento = MetodoPagamento!.Value,
            Itens = itensPedido,
            Subtotal = _carrinhoService.CalcularSubtotal(),
            Frete = _carrinhoService.CalcularFrete(),
            Desconto = desconto,
            CodigoCupom = desconto > 0 ? _carrinhoService.CupomAplicado?.Codigo : null,
            ValorTotal = _carrinhoService.CalcularTotal(),
            Status = StatusPedido.Pendente,
            DataPedido = DateTime.UtcNow
        };

        var itensComEstoqueBaixado = new List<ItemCarrinho>();

        try
        {
            foreach (var item in itensPedido)
            {
                if (!await _produtoService.BaixarEstoqueAsync(item.ProdutoId, item.Quantidade))
                {
                    await DevolverEstoqueAsync(itensComEstoqueBaixado);
                    ModelState.AddModelError(string.Empty, $"Estoque insuficiente para \"{item.Nome}\". Ajuste a quantidade no carrinho.");
                    return Page();
                }

                itensComEstoqueBaixado.Add(item);
            }

            await _pedidoService.CriarAsync(pedido);
        }
        catch (Exception)
        {
            await DevolverEstoqueAsync(itensComEstoqueBaixado);
            ModelState.AddModelError(string.Empty, "Não foi possível finalizar o pedido. Tente novamente.");
            return Page();
        }

        _carrinhoService.Limpar();

        return RedirectToPage(new { pedidoId = pedido.Id });
    }

    private async Task DevolverEstoqueAsync(List<ItemCarrinho> itens)
    {
        foreach (var item in itens)
        {
            try
            {
                await _produtoService.DevolverEstoqueAsync(item.ProdutoId, item.Quantidade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao devolver {Quantidade} unidade(s) do produto {ProdutoId} ao estoque.", item.Quantidade, item.ProdutoId);
            }
        }
    }
}
