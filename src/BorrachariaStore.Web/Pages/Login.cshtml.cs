using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

namespace BorrachariaStore.Web.Pages;

public class LoginModel : PageModel
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public LoginModel(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

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

    private sealed class Credencial
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // TODO: Validar ModelState
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var archivePath = Path.Combine(
            _webHostEnvironment.ContentRootPath,
            "Data",
            "MockData",
            "credencial.json");

        await using var archiveStream = System.IO.File.OpenRead(archivePath);
        var credencial = await JsonSerializer.DeserializeAsync<List<Credencial>>(
            archiveStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (credencial is null)
        {
            throw new InvalidDataException("O arquivo de credenciais não contém uma lista válida.");
        }

        var validCredencial = credencial.Any(credencial =>
            string.Equals(credencial.Email, Email, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(credencial.Password, Senha, StringComparison.Ordinal));

        if (!validCredencial)
        {
            MensagemErro = "E-mail ou senha inválidos.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(
            claims, 
            CookieAuthenticationDefaults.AuthenticationScheme);

        var user = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            user);

        return RedirectToPage("/Admin/Index");
    }
}
