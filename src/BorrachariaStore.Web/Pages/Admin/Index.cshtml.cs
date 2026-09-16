using BorrachariaStore.Web.Models;
using BorrachariaStore.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace BorrachariaStore.Web.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ProdutoService _produtoService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public IndexModel(ProdutoService produtoService, IWebHostEnvironment webHostEnvironment)
    {
        _produtoService = produtoService;
        _webHostEnvironment = webHostEnvironment;
    }

    public List<Produto> Produtos { get; set; } = new();
    public string? MensagemErro { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var productList = await _produtoService.ListarAsync();
            Produtos = productList ?? new List<Produto>();

            if (Produtos.Count == 0)
            {
                ViewData["Aviso"] = "Nenhum produto cadastrado no momento.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERRO] Admin Index OnGetAsync: {ex.Message}");
            Produtos = new List<Produto>();
            MensagemErro = "Falha ao carregar a lista de produtos. Tente novamente mais tarde.";
        }
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
            Console.WriteLine($"[LOG ERRO] Admin Index OnPostRemoverAsync: {ex.Message}");
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
                produto.UrlFoto = existingProduct.UrlFoto;
            }

            // Remove somente as fotos que o usuário marcou para exclusão
            if (fotosParaRemover != null && fotosParaRemover.Count > 0)
            {
                produto.Fotos.RemoveAll(f => fotosParaRemover.Contains(f));
                produto.UrlFoto = produto.Fotos.FirstOrDefault() ?? string.Empty;
            }

            if (uploadedImageFiles != null && uploadedImageFiles.Count > 0)
            {
                string targetFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "produtos");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                foreach (var file in uploadedImageFiles)
                {
                    if (file.Length > 0)
                    {
                        string safeFileName = Path.GetFileName(file.FileName).Replace(" ", "_");
                        string uniqueFileName = Guid.NewGuid().ToString("N")[..8] + "_" + safeFileName;
                        string destinationFilePath = Path.Combine(targetFolder, uniqueFileName);

                        using (var fileStream = new FileStream(destinationFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        string photoPath = $"/img/produtos/{uniqueFileName}";
                        produto.Fotos.Add(photoPath);
                    }
                }

                produto.Fotos = produto.Fotos.Distinct().ToList();

                if (produto.Fotos.Count > 0)
                {
                    produto.UrlFoto = produto.Fotos[0];
                }
            }
            else
            {
                produto.Fotos = produto.Fotos.Distinct().ToList();
            }

            string rawPriceString = Request.Form["Produto.Preco"].ToString();

            if (!string.IsNullOrWhiteSpace(rawPriceString))
            {
                string normalizedPriceString = rawPriceString.Replace(",", ".");

                bool isPriceValid = decimal.TryParse(
                    normalizedPriceString,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out decimal validPrice
                );

                if (isPriceValid)
                {
                    produto.Preco = validPrice;
                }
            }

            await _produtoService.AtualizarAsync(produto.Id, produto);
            TempData["MensagemSucesso"] = "Produto atualizado com sucesso!";
        }

        return RedirectToPage();
    }
}