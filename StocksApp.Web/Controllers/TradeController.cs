using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using ServiceContracts.FinnhubService;
using ServiceContracts.StocksService;
using ServiceContracts.DTO;
using StocksApp.Filters.ActionFilters;
using StocksApp.Models;
using System.Globalization;

namespace StocksApp.Controllers;

[Route("[controller]")]
public class TradeController : Controller
{
    CultureInfo cultureInfo = new("en-US");

    private readonly TradingOptions _tradingOptions;
    private readonly IBuyOrdersService _stocksBuyOrdersService;
    private readonly ISellOrdersService _stocksSellOrdersService;
    private readonly IFinnhubCompanyProfileService _finnhubCompanyProfileService;
    private readonly IFinnhubStockPriceQuoteService _finnhubStockPriceQuoteService;
    private readonly IFinnhubSearchStocksService _finnhubSearchStocksService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TradeController> _logger;

    public TradeController(IOptions<TradingOptions> tradingOptions, IBuyOrdersService stocksBuyOrdersService, ISellOrdersService stocksSellOrdersService, IFinnhubCompanyProfileService finnhubCompanyProfileService, IFinnhubStockPriceQuoteService finnhubStockPriceQuoteService, IFinnhubSearchStocksService finnhubSearchStocksService, IConfiguration configuration, ILogger<TradeController> logger)
    {
        _tradingOptions = tradingOptions.Value;
        _stocksBuyOrdersService = stocksBuyOrdersService;
        _stocksSellOrdersService = stocksSellOrdersService;
        _finnhubCompanyProfileService = finnhubCompanyProfileService;
        _finnhubStockPriceQuoteService = finnhubStockPriceQuoteService;
        _finnhubSearchStocksService = finnhubSearchStocksService;
        _configuration = configuration;
        _logger = logger;
    }

    [Route("[action]/{stockSymbol?}")]
    [Route("~/[controller]/{stockSymbol?}")]
    public async Task<IActionResult> Index(string stockSymbol)
    {
        _logger.LogInformation("In TradeController.Index() action method");
        _logger.LogDebug("stockSymbol: {stockSymbol}", stockSymbol);

        // reset stock symbol if not exists
        if (string.IsNullOrEmpty(stockSymbol))
            stockSymbol = "MSFT";

        // get company profile from API server
        Dictionary<string, object>? profileDictionary = await _finnhubCompanyProfileService.GetCompanyProfile(stockSymbol);
        // get stock price quotes from API server
        Dictionary<string, object>? quoteDictionary = await _finnhubStockPriceQuoteService.GetStockPriceQuote(stockSymbol);

        // create model object
        StockTrade stockTrade = new()
        {
            StockSymbol = stockSymbol
        };

        // load data from finnhubService into model object
        if (profileDictionary != null && quoteDictionary != null)
        {
            stockTrade = new StockTrade()
            {
                StockSymbol = profileDictionary["ticker"].ToString(),
                StockName = profileDictionary["name"].ToString(),
                Quantity = _tradingOptions.DefaultOrderQuantity ?? 0, 
                Price = Convert.ToDouble(quoteDictionary["c"].ToString(), cultureInfo)
            };
        }

        // send Finnhub token to view
        ViewBag.FinnhubToken = _configuration["FinnhubToken"];

        return View(stockTrade);
    }

    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(CreateOrderActionFilter))]
    public async Task<IActionResult> BuyOrder(BuyOrderRequest orderRequest)
    {
        BuyOrderResponse buyOrderResponse = await _stocksBuyOrdersService.CreateBuyOrder(orderRequest);

        return RedirectToAction(nameof(Orders));
    }

    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(CreateOrderActionFilter))]
    public async Task<IActionResult> SellOrder(SellOrderRequest orderRequest)
    {
        SellOrderResponse sellOrderResponse = await _stocksSellOrdersService.CreateSellOrder(orderRequest);

        return RedirectToAction(nameof(Orders));
    }

    [Route("[action]")]
    public async Task<IActionResult> Orders()
    {
        List<BuyOrderResponse> buyOrderResponses = await _stocksBuyOrdersService.GetBuyOrders();
        List<SellOrderResponse> sellOrderResponses = await _stocksSellOrdersService.GetSellOrders();

        Orders orders = new()
        {
            BuyOrders = buyOrderResponses,
            SellOrders = sellOrderResponses
        };

        ViewBag.TradingOptions = _tradingOptions;

        return View(orders);
    }

    [Route("OrdersPDF")]
    public async Task<IActionResult> OrdersPDF()
    {
        List<IOrderResponse> orders = new();

        orders.AddRange(await _stocksBuyOrdersService.GetBuyOrders());
        orders.AddRange(await _stocksSellOrdersService.GetSellOrders());

        orders = orders.OrderByDescending(temp => temp.DateAndTimeOfOrder).ToList();

        ViewBag.TradingOptions = _tradingOptions;

        return new ViewAsPdf("OrdersPDF", orders, ViewData)
        {
            PageMargins = new Rotativa.AspNetCore.Options.Margins()
            {
                Top = 20,
                Right = 20,
                Bottom = 20,
                Left = 20
            },
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        };
    }
}
