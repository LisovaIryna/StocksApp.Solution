using Exceptions;
using RepositoryContracts;
using ServiceContracts.FinnhubService;

namespace Services.FinnhubService;

public class FinnhubStockPriceQuoteService : IFinnhubStockPriceQuoteService
{
    private readonly IFinnhubRepository _finnhubRepository;

    public FinnhubStockPriceQuoteService(IFinnhubRepository finnhubRepository)
    {
        _finnhubRepository = finnhubRepository;
    }

    public async Task<Dictionary<string, object>?> GetStockPriceQuote(string stockSymbol)
    {
        try
        {
            return await _finnhubRepository.GetStockPriceQuote(stockSymbol);
        }
        catch (Exception ex)
        {
            throw new FinnhubException("Unable to connect to finnhub", ex);
        }
    }
}
