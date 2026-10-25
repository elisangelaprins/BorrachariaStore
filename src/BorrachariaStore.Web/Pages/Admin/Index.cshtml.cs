using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace BorrachariaStore.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private const int LimiteBaixoEstoque = 5;
    private readonly HashSet<string> allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif" };
    private readonly ProdutoService _produtoService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ProdutoService produtoService, IWebHostEnvironment webHostEnvironment, ILogger<IndexModel> logger)
    {
        _produtoService = produtoService;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    public List<Produto> Produtos { get; set; } = new();
    public string? MensagemErro { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Busca { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? Aro { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    public bool FiltroAtivo => !string.IsNullOrWhiteSpace(Busca) || Aro.HasValue || !string.IsNullOrWhiteSpace(Status);

    public async Task OnGetAsync()
    {
        try
        {
            var productList = await _produtoService.BuscarAsync(Busca, null);
            IEnumerable<Produto> filtrados = productList ?? new List<Produto>();

            if (Aro.HasValue)
            {
                filtrados = filtrados.Where(p => p.Aro == Aro.Value);
            }

            filtrados = Status switch
            {
                "Ativo" => filtrados.Where(p => p.Estoque > 0),
                "BaixoEstoque" => filtrados.Where(p => p.Estoque > 0 && p.Estoque <= LimiteBaixoEstoque),
                "Inativo" => filtrados.Where(p => p.Estoque <= 0),
                _ => filtrados
            };

            Produtos = filtrados.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao carregar a lista de produtos no painel administrativo.");
            Produtos = new List<Produto>();
            MensagemErro = "Falha ao carregar a lista de produtos. Tente novamente mais tarde.";
        }
    }

    public async Task<IActionResult> OnPostSairAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Login");
    }

    public async Task<IActionResult> OnPostRemoverAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return RedirectToPage();
        }

        try
        {
            await _produtoService.RemoverAsync(id);
            TempData["MensagemSucesso"] = "Produto excluído com sucesso!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar excluir o produto {ProdutoId} no painel administrativo.", id);
            TempData["MensagemErro"] = "Ocorreu um erro ao tentar excluir o produto.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditarAsync(Produto produto, List<IFormFile>? uploadedImageFiles, List<string>? fotosParaRemover)
    {
        if (produto != null && !string.IsNullOrWhiteSpace(produto.Id))
        {
            // Busca o produto original para não perder fotos antigas caso não envie novas
            var existingProduct = await _produtoService.ObterPorIdAsync(produto.Id);
            if (existingProduct != null)
            {
                produto.Fotos = existingProduct.Fotos ?? new List<string>();
                produto.Avaliacao = existingProduct.Avaliacao;
            }

            // Remove somente as fotos que o usuário marcou para exclusão
            if (fotosParaRemover != null && fotosParaRemover.Count > 0)
            {
                produto.Fotos.RemoveAll(f => fotosParaRemover.Contains(f));
            }

            if (uploadedImageFiles != null && uploadedImageFiles.Count > 0)
            {
                string targetFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "produtos");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                foreach (var file in uploadedImageFiles)
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    if (file.Length > 0 && allowedExtensions.Contains(fileExtension))
                    {
                        string safeFileName = Path.GetFileName(file.FileName).Replace(" ", "_");
                        string uniqueFileName = Guid.NewGuid().ToString("N")[..8] + "_" + safeFileName;
                        string destinationFilePath = Path.Combine(targetFolder, uniqueFileName);

                        using (var fileStream = new FileStream(destinationFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        string photoPath = $"/images/produtos/{uniqueFileName}";
                        produto.Fotos.Add(photoPath);
                    }
                }
            }

            produto.Fotos = produto.Fotos.Distinct().ToList();

            string rawNormal = Request.Form["PrecoNormal"].ToString();
            decimal precoNormal = produto.Preco;
            if (!string.IsNullOrWhiteSpace(rawNormal))
            {
                string norm = rawNormal.Replace(",", ".");
                if (decimal.TryParse(norm, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal validNormal) && validNormal > 0)
                {
                    precoNormal = validNormal;
                }
            }

            string rawPromo = Request.Form["PrecoPromocional"].ToString();
            decimal? precoPromo = null;
            if (!string.IsNullOrWhiteSpace(rawPromo))
            {
                string norm = rawPromo.Replace(",", ".");
                if (decimal.TryParse(norm, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal validPromo) && validPromo > 0)
                {
                    precoPromo = validPromo;
                }
            }

            if (precoPromo.HasValue && precoPromo.Value < precoNormal && precoNormal > 0)
            {
                produto.Preco = precoPromo.Value;
                produto.PrecoOriginal = precoNormal;
                produto.EmPromocao = true;
                produto.PorcentagemDesconto = (int)Math.Round((1m - (precoPromo.Value / precoNormal)) * 100m);
            }
            else
            {
                produto.Preco = precoNormal;
                produto.PrecoOriginal = null;
                produto.EmPromocao = false;
                produto.PorcentagemDesconto = null;
            }

            await _produtoService.AtualizarAsync(produto.Id, produto);
            TempData["MensagemSucesso"] = "Produto atualizado com sucesso!";
        }

        return RedirectToPage();
    }
}
