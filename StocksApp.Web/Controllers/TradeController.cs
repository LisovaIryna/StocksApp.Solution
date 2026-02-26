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

        if (string.IsNullOrEmpty(stockSymbol))
            stockSymbol = "MSFT";

        Dictionary<string, object>? profileDictionary = await _finnhubCompanyProfileService.GetCompanyProfile(stockSymbol);
        Dictionary<string, object>? quoteDictionary = await _finnhubStockPriceQuoteService.GetStockPriceQuote(stockSymbol);

        StockTrade stockTrade = new()
        {
            StockSymbol = stockSymbol
        };

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

        ViewBag.FinnhubToken = _configuration["FinnhubToken"];

        return View(stockTrade);
    }

    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(CreateOrderActionFilter))]
    public async Task<IActionResult> BuyOrder(BuyOrderRequest buyOrderRequest)
    {
        BuyOrderResponse buyOrderResponse = await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);

        return RedirectToAction(nameof(Orders));
    }

    [Route("[action]")]
    [HttpPost]
    [TypeFilter(typeof(CreateOrderActionFilter))]
    public async Task<IActionResult> SellOrder(SellOrderRequest sellOrderRequest)
    {
        SellOrderResponse sellOrderResponse = await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);

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
