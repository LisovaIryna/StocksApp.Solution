namespace ServiceContracts.FinnhubService;

/// <summary>
/// Represents a service that makes HTTP requests to finnhub.io
/// </summary>
public interface IFinnhubCompanyProfileService
{
    /// <summary>
    /// Returns company details
    /// </summary>
    /// <param name="stockSymbol">Stock symbol to search</param>
    /// <returns>Dictionary that contains company details</returns>
    Task<Dictionary<string, object>?> GetCompanyProfile(string stockSymbol);
}
