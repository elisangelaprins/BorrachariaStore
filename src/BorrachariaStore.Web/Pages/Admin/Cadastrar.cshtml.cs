using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorrachariaStore.Web.Pages.Admin;

public class CadastrarModel : PageModel
{
    private readonly ProdutoService _produtoService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CadastrarModel(ProdutoService produtoService, IWebHostEnvironment webHostEnvironment)
    {
        _produtoService = produtoService;
        _webHostEnvironment = webHostEnvironment;
    }

    [BindProperty]
    
    public Produto produto { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(List <IFormFile> uploadedImageFiles)
    {
        if (!ModelState.IsValid) {
            return Page();
        }

        if (uploadedImageFiles != null && uploadedImageFiles.Count > 0) {
            string targetFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "produtos");
            if (!Directory.Exists(targetFolder)) {
                Directory.CreateDirectory(targetFolder);
            }

            foreach (var file in uploadedImageFiles) {
                if (file.Length > 0) {
                    string safeFileName = Path.GetFileName(file.FileName).Replace(" ", "_") ;
                    string uniqueFileName = Guid.NewGuid().ToString("N")[..8] + "_" + safeFileName;
                    string destinationFilePath = Path.Combine(targetFolder, uniqueFileName);

                    using (var fileStream = new FileStream(destinationFilePath, FileMode.Create)) {
                        await file.CopyToAsync(fileStream);
                    }

                    string photoPath = $"/img/produtos/{uniqueFileName}";
                    produto.Fotos.Add(photoPath);
                }
            }

            produto.Fotos = produto.Fotos.Distinct().ToList();

            if (produto.Fotos.Count > 0) {
                produto.UrlFoto = produto.Fotos[0];
            }
        }

        await _produtoService.CriarAsync(produto);
        return RedirectToPage("/Admin/Index");
    }
}
