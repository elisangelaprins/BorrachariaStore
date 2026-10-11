using System.Net.Http.Json;
using System.Text.RegularExpressions;
using BorrachariaStore.Web.Models;
using Microsoft.Extensions.Logging;

namespace BorrachariaStore.Web.Services;

public class ViaCepService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ViaCepService> _logger;

    public ViaCepService(IHttpClientFactory httpClientFactory, ILogger<ViaCepService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<EnderecoCepDto?> BuscarEnderecoPorCepAsync(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep)) return null;

        var cleanCep = Regex.Replace(cep, @"[^\d]", "");

        if (cleanCep.Length != 8) return null;

        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            var url = $"https://viacep.com.br/ws/{cleanCep}/json/";

            var result = await httpClient.GetFromJsonAsync<EnderecoCepDto>(url);

            if (result != null && result.Erro == "true")
            {
                return null;
            }

            return result;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Tempo limite de 5s excedido ao consultar o CEP {Cep} no ViaCEP.", cleanCep);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Falha de conexão com a API do ViaCEP ao consultar CEP {Cep}.", cleanCep);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar o CEP {Cep} no ViaCepService.", cleanCep);
            return null;
        }
    }
}