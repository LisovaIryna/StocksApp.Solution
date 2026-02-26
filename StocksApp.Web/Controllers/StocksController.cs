using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceContracts;
using ServiceContracts.FinnhubService;
using StocksApp.Models;

namespace StocksApp.Controllers;

[Route("[controller]")]
public class StocksController : Controller
{
    private readonly TradingOptions _tradingOptions;
    private readonly IFinnhubStocksService _finnhubStocksService;
    private readonly ILogger<StocksController> _logger;

    public StocksController(IOptions<TradingOptions> tradingOptions, IFinnhubStocksService finnhubStocksService, ILogger<StocksController> logger)
    {
        _tradingOptions = tradingOptions.Value;
        _finnhubStocksService = finnhubStocksService;
        _logger = logger;
    }

    [Route("/")]
    [Route("[action]/{stock?}")]
    [Route("~/[action]/{stock?}")]
    public async Task<IActionResult> Explore(string? stock, bool showAll = false)
    {
        _logger.LogInformation("In StocksController.Explore() action method");
        _logger.LogDebug("stock: {stock}, showAll: {showAll}", stock, showAll);

        List<Dictionary<string, string>>? stocksDictionary = await _finnhubStocksService.GetStocks();
        List<Stock> stocks = new();

        if (stocksDictionary is not null)
        {
            if (!showAll && _tradingOptions.Top25PopularStocks != null)
            {
                string[]? Top25PopularStocksList = _tradingOptions.Top25PopularStocks.Split(",");
                if (Top25PopularStocksList is not null)
                {
                    stocksDictionary = stocksDictionary
                        .Where(temp => Top25PopularStocksList.Contains(Convert.ToString(temp["symbol"]))).ToList();
                }
            }

            stocks = stocksDictionary
                .Select(temp => new Stock()
                {
                    StockName = Convert.ToString(temp["description"]),
                    StockSymbol = Convert.ToString(temp["symbol"])
                }).ToList();
        }

        ViewBag.Stock = stock;

        return View(stocks);
    }
}
