namespace RepositoryContracts;

/// <summary>
/// Represents a repository that makes HTTP requests to finnhub.io
/// </summary>
public interface IFinnhubRepository
{
    /// <summary>
    /// Returns company details
    /// </summary>
    /// <param name="stockSymbol">Stock symbol to search</param>
    /// <returns>Dictionary that contains company details</returns>
    Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol);

    /// <summary>
    /// Returns stock price details
    /// </summary>
    /// <param name="stockSymbol">Stock symbol to search</param>
    /// <returns>Dictionary that contains stock price details</returns>
    Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol);

    /// <summary>
    /// Returns list of all stocks supported by an exchange
    /// </summary>
    /// <returns>List of stocks</returns>
    Task<List<Dictionary<string, string>>?> GetStocks();

    /// <summary>
    /// Returns list of matching stocks based on the given stock symbol
    /// </summary>
    /// <param name="stockSymbolToSearch">Stock symbol to search</param>
    /// <returns>List of matching stocks</returns>
    Task<Dictionary<string, object>?> SearchStocks(string stockSymbolToSearch);
}
