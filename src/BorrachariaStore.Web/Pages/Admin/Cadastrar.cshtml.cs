using System.Globalization;
using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorrachariaStore.Web.Pages.Admin;

public class CadastrarModel : PageModel
{
    private readonly HashSet<string> allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif" };
    private readonly ProdutoService _produtoService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CadastrarModel(ProdutoService produtoService, IWebHostEnvironment webHostEnvironment)
    {
        _produtoService = produtoService;
        _webHostEnvironment = webHostEnvironment;
    }

    [BindProperty]
    
    public Produto Produto { get; set; } = new();

    [BindProperty]
    public decimal PrecoNormal { get; set; }

    [BindProperty]
    public decimal? PrecoPromocional { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(List <IFormFile> uploadedImageFiles)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (uploadedImageFiles != null && uploadedImageFiles.Count > 0) {
            string targetFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "produtos");
            if (!Directory.Exists(targetFolder)) {
                Directory.CreateDirectory(targetFolder);
            }

            foreach (var file in uploadedImageFiles) {
                var fileExtension = Path.GetExtension(file.FileName);
                if (file.Length > 0 && allowedExtensions.Contains(fileExtension)) {
                    string safeFileName = Path.GetFileName(file.FileName).Replace(" ", "_") ;
                    string uniqueFileName = Guid.NewGuid().ToString("N")[..8] + "_" + safeFileName;
                    string destinationFilePath = Path.Combine(targetFolder, uniqueFileName);

                    using (var fileStream = new FileStream(destinationFilePath, FileMode.Create)) {
                        await file.CopyToAsync(fileStream);
                    }

                    string photoPath = $"/images/produtos/{uniqueFileName}";
                    Produto.Fotos.Add(photoPath);
                }
            }

            Produto.Fotos = Produto.Fotos.Distinct().ToList();

            if (Produto.Fotos.Count > 0 && string.IsNullOrEmpty(Produto.UrlFoto)) {
                Produto.UrlFoto = Produto.Fotos[0];
            }
        }
        
        string rawNormalPrice = Request.Form["PrecoNormal"];
        if (!string.IsNullOrWhiteSpace(rawNormalPrice)) {
            string normalizedNormalPrice = rawNormalPrice.Replace(",", ".");
            if (decimal.TryParse(normalizedNormalPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal validNormalPrice) && validNormalPrice > 0) {
                PrecoNormal = validNormalPrice;
            }
        }

        string rawPromoPrice = Request.Form["PrecoPromocional"];
        if (!string.IsNullOrWhiteSpace(rawPromoPrice)) {
            string normalizedPromoPrice = rawPromoPrice.Replace(",", ".");
            if (decimal.TryParse(normalizedPromoPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal validPromoPrice) && validPromoPrice > 0) {
                PrecoPromocional = validPromoPrice;
            }
        }

        if (PrecoPromocional.HasValue && PrecoPromocional.Value > 0 && PrecoPromocional.Value < PrecoNormal) {
            Produto.Preco = PrecoPromocional.Value;
            Produto.PrecoOriginal = PrecoNormal;
            Produto.EmPromocao = true;
            Produto.PorcentagemDesconto = (int)Math.Round((1m - (PrecoPromocional.Value / PrecoNormal)) * 100m);
        } else {
            Produto.Preco = PrecoNormal;
            Produto.PrecoOriginal = null;
            Produto.EmPromocao = false;
            Produto.PorcentagemDesconto = null;
        }

        try
        {
            await _produtoService.CriarAsync(Produto);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Não foi possível cadastrar o produto. Tente novamente.");
            return Page();
        }

        TempData["MensagemSucesso"] = "Produto cadastrado com sucesso!";
        return RedirectToPage("/Admin/Index");
    }
}
