using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BorrachariaStore.Web.Pages;

public class LoginModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Senha { get; set; } = string.Empty;

    public string? MensagemErro { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // TODO: Validar ModelState
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // TODO: O integrante implementará a autenticação no backend:
        // - Validar credenciais no banco de dados ou Identity
        // - Criar Cookie de autenticação ou Sessão do usuário logado
        // - Redirecionar para /Admin/Index apenas em caso de sucesso:
        // return RedirectToPage("/Admin/Index");

        return Page();
    }
}
