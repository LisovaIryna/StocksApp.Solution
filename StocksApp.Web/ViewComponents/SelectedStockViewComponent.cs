using Microsoft.AspNetCore.Mvc;
using ServiceContracts.FinnhubService;
using ServiceContracts.StocksService;
using Microsoft.Extensions.Options;

namespace StocksApp.ViewComponents;

public class SelectedStockViewComponent :ViewComponent
{
    private readonly TradingOptions _tradingOptions;
    private readonly IBuyOrdersService _stocksBuyOrdersService;
    private readonly IFinnhubCompanyProfileService _finnhubCompanyProfileService;
    private readonly IFinnhubStockPriceQuoteService _finnhubStockPriceQuoteService;
    private readonly IConfiguration _configuration;

    public SelectedStockViewComponent (IOptions<TradingOptions> tradingOptions, IBuyOrdersService stocksBuyOrdersService, IFinnhubCompanyProfileService finnhubCompanyProfileService, IFinnhubStockPriceQuoteService finnhubStockPriceQuoteService, IConfiguration configuration)
    {
        _tradingOptions = tradingOptions.Value;
        _stocksBuyOrdersService = stocksBuyOrdersService;
        _finnhubCompanyProfileService = finnhubCompanyProfileService;
        _finnhubStockPriceQuoteService = finnhubStockPriceQuoteService;
        _configuration = configuration;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? stockSymbol)
    {
        Dictionary<string, object>? profitDictionary = null;

        if (stockSymbol != null)
        {
            profitDictionary = await _finnhubCompanyProfileService.GetCompanyProfile(stockSymbol);
            var stockPriceDictionary = await _finnhubStockPriceQuoteService.GetStockPriceQuote(stockSymbol);
            if (stockPriceDictionary != null && profitDictionary != null)
                profitDictionary.Add("price", stockPriceDictionary["c"]);
        }


        if (profitDictionary != null && profitDictionary.ContainsKey("logo"))
            return View(profitDictionary);
        else
            return Content("");
    }
}
