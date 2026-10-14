using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace BorrachariaStore.Web.Pages;

public class CarrinhoModel : PageModel
{
    private readonly CarrinhoService _carrinhoService;
    private readonly CupomService _cupomService;
    private readonly ILogger<CarrinhoModel> _logger;

    public CarrinhoModel(CarrinhoService carrinhoService, CupomService cupomService, ILogger<CarrinhoModel> logger)
    {
        _carrinhoService = carrinhoService;
        _cupomService = cupomService;
        _logger = logger;
    }

    public IReadOnlyList<ItemCarrinho> Itens => _carrinhoService.Itens;

    public decimal Subtotal => _carrinhoService.CalcularSubtotal();

    public decimal Desconto => _carrinhoService.CalcularDesconto();

    public Cupom? Cupom => _carrinhoService.CupomAplicado;

    public decimal Frete => _carrinhoService.CalcularFrete();

    public decimal Total => _carrinhoService.CalcularTotal();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAplicarCupomAsync(string cupom)
    {
        if (_carrinhoService.Itens.Count == 0)
        {
            TempData["MensagemErroCupom"] = "Adicione produtos antes de aplicar um cupom.";
            return RedirectToPage();
        }

        var resultado = await _cupomService.ValidarCupomAsync(cupom, Subtotal);
        if (resultado.Valido && resultado.Cupom != null)
        {
            _carrinhoService.AplicarCupom(resultado.Cupom);
            TempData["MensagemSucessoCupom"] = resultado.Mensagem;
        }
        else
        {
            TempData["MensagemErroCupom"] = resultado.Mensagem;
        }

        return RedirectToPage();
    }

    public IActionResult OnPostRemoverCupom()
    {
        _carrinhoService.RemoverCupom();
        TempData["MensagemSucessoCupom"] = "Cupom removido.";
        return RedirectToPage();
    }

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
            _logger.LogError(ex, "Erro ao atualizar quantidade do produto {ProdutoId} no carrinho.", produtoId);
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
            _logger.LogError(ex, "Erro ao remover produto {ProdutoId} do carrinho.", produtoId);
        }

        return RedirectToPage();
    }
}
