namespace ServiceContracts.FinnhubService;

/// <summary>
/// Represents a service that makes HTTP requests to finnhub.io
/// </summary>
public interface IFinnhubStockPriceQuoteService
{
    /// <summary>
    /// Returns stock price details
    /// </summary>
    /// <param name="stockSymbol">Stock symbol to search</param>
    /// <returns>Dictionary that contains stock price details</returns>
    Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol);
}
