using AutoFixture;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ServiceContracts;
using ServiceContracts.FinnhubService;
using StocksApp;
using StocksApp.Controllers;
using StocksApp.Models;

namespace Tests.ControllerTests;

public class StocksControllerTest
{
    private readonly IFinnhubStocksService _finnhubStocksService;
    private readonly Mock<IFinnhubStocksService> _finnhubStocksServiceMock;
    private readonly Mock<ILogger<StocksController>> _loggerMock;
    private readonly Fixture _fixture;

    public StocksControllerTest()
    {
        _fixture = new Fixture();
        _finnhubStocksServiceMock = new Mock<IFinnhubStocksService>();
        _loggerMock = new Mock<ILogger<StocksController>>();
        _finnhubStocksService = _finnhubStocksServiceMock.Object;
    }

    #region Explore

    [Fact]
    public async Task Explore_StocksIsNull_ShouldReturnExploreViewWithStockList()
    {
        // Arrange
        IOptions<TradingOptions> tradingOptions = Options.Create(new TradingOptions()
        {
            DefaultOrderQuantity = 100,
            Top25PopularStocks = "AAPL,MSFT,AMZN,TSLA,GOOGL,GOOG,NVDA,BRK.B,META,UNH,JNJ,JPM,V,PG,XOM,HD,CVX,MA,BAC,ABBV,PFE,AVGO,COST,DIS,KO"
        });

        StocksController stocksController = new(tradingOptions, _finnhubStocksService, _loggerMock.Object);

        List<Dictionary<string, string>>? stocksDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(@"[{'currency':'USD','description':'APPLE INC','displaySymbol':'AAPL','figi':'BBG000B9XRY4','isin':null,'mic':'XNAS','shareClassFIGI':'BBG001S5N8V8','symbol':'AAPL','symbol2':'','type':'Common Stock'}, {'currency':'USD','description':'MICROSOFT CORP','displaySymbol':'MSFT','figi':'BBG000BPH459','isin':null,'mic':'XNAS','shareClassFIGI':'BBG001S5TD05','symbol':'MSFT','symbol2':'','type':'Common Stock'}, {'currency':'USD','description':'AMAZON.COM INC','displaySymbol':'AMZN','figi':'BBG000BVPV84','isin':null,'mic':'XNAS','shareClassFIGI':'BBG001S5PQL7','symbol':'AMZN','symbol2':'','type':'Common Stock'}, {'currency':'USD','description':'TESLA INC','displaySymbol':'TSLA','figi':'BBG000N9MNX3','isin':null,'mic':'XNAS','shareClassFIGI':'BBG001SQKGD7','symbol':'TSLA','symbol2':'','type':'Common Stock'}, {'currency':'USD','description':'ALPHABET INC-CL A','displaySymbol':'GOOGL','figi':'BBG009S39JX6','isin':null,'mic':'XNAS','shareClassFIGI':'BBG009S39JY5','symbol':'GOOGL','symbol2':'','type':'Common Stock'}]");

        _finnhubStocksServiceMock.Setup(temp => temp.GetStocks())
            .ReturnsAsync(stocksDictionary);

        var expectedStocks = stocksDictionary!.Select(temp => new Stock()
        {
            StockName = Convert.ToString(temp["description"]),
            StockSymbol = Convert.ToString(temp["symbol"])
        }).ToList();

        // Act
        IActionResult result = await stocksController.Explore(null, true);

        // Assert
        ViewResult viewResult = Assert.IsType<ViewResult>(result);
        viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<Stock>>();
        viewResult.ViewData.Model.Should().BeEquivalentTo(expectedStocks);
    }

    #endregion
}
