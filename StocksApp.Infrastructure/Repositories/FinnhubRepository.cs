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

        // create http client
        HttpClient httpClient = _httpClientFactory.CreateClient();

        // create http request
        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/stock/profile2?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        // send request
        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        // read response body
        string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
        _diagnosticContext.Set("Response from finnhub", responseBody);

        // convert response body (from JSON into Dictionary)
        Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

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

        // create http client
        HttpClient httpClient = _httpClientFactory.CreateClient();

        // create http request
        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        // send request
        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        // read response body
        string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
        _diagnosticContext.Set("Response from finnhub", responseBody);

        // convert response body (from JSON into Dictionary)
        Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

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

        // create http client
        HttpClient httpClient = _httpClientFactory.CreateClient();

        // create http request
        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/stock/symbol?exchange=US&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        // send request
        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        // read response body
        string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

        // convert response body (from JSON into Dictionary)
        List<Dictionary<string, string>>? responseDictionary = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(responseBody);

        if (responseDictionary == null)
            throw new InvalidOperationException("No response from finnhub server");

        return responseDictionary;
    }

    public async Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch)
    {
        // Log
        _logger.LogInformation("In {ClassName}.{MethodName}", nameof(FinnhubRepository), nameof(SearchStocks));

        // create http client
        HttpClient httpClient = _httpClientFactory.CreateClient();

        // create http request
        HttpRequestMessage httpRequestMessage = new()
        {
            RequestUri = new Uri($"https://finnhub.io/api/v1/search?q={stockSymbolToSearch}&token={_configuration["FinnhubToken"]}"),
            Method = HttpMethod.Get
        };

        // send request
        HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

        // read response body
        string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();
        _diagnosticContext.Set("Response from finnhub", responseBody);

        // convert response body (from JSON into Dictionary)
        Dictionary<string, object>? responseDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

        if (responseDictionary == null)
            throw new InvalidOperationException("No response from finnhub server");
        if (responseDictionary.ContainsKey("error"))
            throw new InvalidOperationException(Convert.ToString(responseDictionary["error"]));

        return responseDictionary;
    }
}
