using System.Net.Http.Json;
using System.Text.RegularExpressions;
using BorrachariaStore.Web.Models;

namespace BorrachariaStore.Web.Services;

public class ViaCepService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ViaCepService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<EnderecoCepDto?> BuscarEnderecoPorCepAsync(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep)) return null;

        var cleanCep = Regex.Replace(cep, @"[^\d]", "");

        if(cleanCep.Length != 8) return null;

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var url = $"https://viacep.com.br/ws/{cleanCep}/json/";

            var result = await httpClient.GetFromJsonAsync<EnderecoCepDto>(url);

            if (result != null && result.Erro == "true")
            {
                return null;
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[LOG ERRO REDE] Falha ao comunicar com ViaCEP: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG ERRO] Erro inesperado no ViaCepService: {ex.Message}");
            return null;
        }
    }
    
}