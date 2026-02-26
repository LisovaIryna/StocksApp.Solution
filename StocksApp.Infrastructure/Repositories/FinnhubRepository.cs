using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using System.Text.Json;

namespace Repositories;

public class FinnhubRepository : IFinnhubRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FinnhubRepository> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public FinnhubRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<FinnhubRepository> logger, IDiagnosticContext diagnosticContext)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    public async Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol)
    {
        // Log
        _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(GetCompanyProfile));

        HttpClient httpClient = _httpClientFactory.CreateClient();

        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        string response = await httpResponseMessage.Content.ReadAsStringAsync();
        _diagnosticContext.Set("Response from finnhub", response);

        Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        if (responseDictionary == null)
            throw new InvalidOperationException("No response from finnhub server");
        if (responseDictionary.ContainsKey("error"))
            throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));

        return responseDictionary;
    }

    public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
    {
        // Log
        _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(GetStockPriceQuote));

        HttpClient httpClient = _httpClientFactory.CreateClient();

        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        string response = await httpResponseMessage.Content.ReadAsStringAsync();
        _diagnosticContext.Set("Response from finnhub", response);

        Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        if (responseDictionary == null)
            throw new InvalidOperationException("No reponse from finnhub server");
        if (responseDictionary.ContainsKey("error"))
            throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));

        return responseDictionary;
    }

    public async Task<List<Dictionary<string, string>>?> GetStocks()
    {
        // Log
        _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(GetStocks));

        HttpClient httpClient = _httpClientFactory.CreateClient();

        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/stock/symbol?exchange=US&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        string response = await httpResponseMessage.Content.ReadAsStringAsync();

        List<Dictionary<string, string>>? responseDictionary = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(response);

        if (responseDictionary == null)
            throw new InvalidOperationException("No response from finnhub server");

        return responseDictionary;
    }

    public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
    {
        // Log
        _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(SearchStocks));

        HttpClient httpClient = _httpClientFactory.CreateClient();

        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/search?q={stockSymbolToSearch}&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        string response = await httpResponseMessage.Content.ReadAsStringAsync();
        _diagnosticContext.Set("Response from finnhub", response);

        Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

        if (responseDictionary == null)
            throw new InvalidOperationException("No response from finnhub server");
        if (responseDictionary.ContainsKey("error"))
            throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));

        return responseDictionary;
    }
}
