using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BorrachariaStore.Web.Pages.Admin;

public class CuponsModel : PageModel
{
    private readonly CupomService _cupomService;

    public CuponsModel(CupomService cupomService)
    {
        _cupomService = cupomService;
    }

    public List<Cupom> Cupons { get; set; } = new();

    [BindProperty]
    [Required(ErrorMessage = "O código do cupom é obrigatório.")]
    public string Codigo { get; set; } = string.Empty;

    [BindProperty]
    [Range(1, 100, ErrorMessage = "O desconto deve ser entre 1% e 100%.")]
    public int PorcentagemDesconto { get; set; } = 10;

    [BindProperty]
    public decimal? ValorMinimoPedido { get; set; }

    [BindProperty]
    public DateTime? Validade { get; set; }

    public async Task OnGetAsync()
    {
        Cupons = await _cupomService.ListarTodosAsync();
    }

    public async Task<IActionResult> OnPostCriarAsync()
    {
        if (!ModelState.IsValid)
        {
            Cupons = await _cupomService.ListarTodosAsync();
            return Page();
        }

        var existente = await _cupomService.ObterPorCodigoAsync(Codigo);
        if (existente != null)
        {
            TempData["MensagemErro"] = $"Já existe um cupom com o código \"{Codigo.ToUpperInvariant()}\".";
            return RedirectToPage();
        }

        var novoCupom = new Cupom
        {
            Codigo = Codigo.Trim().ToUpperInvariant(),
            PorcentagemDesconto = PorcentagemDesconto,
            ValorMinimoPedido = ValorMinimoPedido,
            Validade = Validade.HasValue ? Validade.Value.Date.AddDays(1).AddSeconds(-1) : null,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        await _cupomService.CriarAsync(novoCupom);
        TempData["MensagemSucesso"] = $"Cupom \"{novoCupom.Codigo}\" criado com sucesso!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAlternarStatusAsync(string id, bool ativo)
    {
        if (string.IsNullOrWhiteSpace(id)) return RedirectToPage();

        await _cupomService.AlternarStatusAsync(id, ativo);
        TempData["MensagemSucesso"] = ativo ? "Cupom ativado com sucesso!" : "Cupom desativado com sucesso!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostExcluirAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return RedirectToPage();

        await _cupomService.ExcluirAsync(id);
        TempData["MensagemSucesso"] = "Cupom excluído com sucesso!";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditarAsync(
        string editId,
        string editCodigo,
        int editPorcentagemDesconto,
        decimal? editValorMinimoPedido,
        DateTime? editValidade)
    {
        if (string.IsNullOrWhiteSpace(editId) || string.IsNullOrWhiteSpace(editCodigo))
        {
            TempData["MensagemErro"] = "Código do cupom é obrigatório.";
            return RedirectToPage();
        }

        var existente = await _cupomService.ObterPorCodigoAsync(editCodigo);
        if (existente != null && existente.Id != editId)
        {
            TempData["MensagemErro"] = $"Já existe outro cupom com o código \"{editCodigo.Trim().ToUpperInvariant()}\".";
            return RedirectToPage();
        }

        var cupons = await _cupomService.ListarTodosAsync();
        var cupomAtual = cupons.FirstOrDefault(c => c.Id == editId);
        if (cupomAtual == null)
        {
            TempData["MensagemErro"] = "Cupom não encontrado.";
            return RedirectToPage();
        }

        cupomAtual.Codigo = editCodigo.Trim().ToUpperInvariant();
        cupomAtual.PorcentagemDesconto = Math.Clamp(editPorcentagemDesconto, 1, 100);
        cupomAtual.ValorMinimoPedido = editValorMinimoPedido;
        cupomAtual.Validade = editValidade.HasValue ? editValidade.Value.Date.AddDays(1).AddSeconds(-1) : null;

        await _cupomService.AtualizarAsync(cupomAtual);
        TempData["MensagemSucesso"] = $"Cupom \"{cupomAtual.Codigo}\" atualizado com sucesso!";
        return RedirectToPage();
    }
}
