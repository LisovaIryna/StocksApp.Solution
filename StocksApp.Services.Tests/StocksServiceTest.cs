using AutoFixture;
using Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts.StocksService;
using ServiceContracts.DTO;
using Services;
using Services.StocksService;

namespace Tests.ServiceTests;

public class StocksServiceTest
{
    private readonly IBuyOrdersService _stocksBuyOrdersService;
    private readonly ISellOrdersService _stocksSellOrdersService;
    private readonly Mock<IStocksRepository> _stocksRepositoryMock;
    private readonly IStocksRepository _stocksRepository;
    private readonly IFixture _fixture;

    public StocksServiceTest()
    {
        _fixture = new Fixture();
        _stocksRepositoryMock = new();
        _stocksRepository = _stocksRepositoryMock.Object;
        _stocksBuyOrdersService = new StocksBuyOrdersService(_stocksRepository);
        _stocksSellOrdersService = new StocksSellOrdersService(_stocksRepository);
    }

    #region CreateBuyOrder

    [Fact]
    public async Task CreateBuyOrder_NullBuyOrder_ToBeArgumentNullException()
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = null;

        // Mock
        BuyOrder buyOrder = _fixture.Build<BuyOrder>().Create();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    public async Task CreateBuyOrder_BuyOrderQuantityIsLessThanMinimum_ToBeArgumentException(uint buyOrderQuantity)
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = _fixture.Build<BuyOrderRequest>()
            .With(temp => temp.Quantity, buyOrderQuantity)
            .Create();

        // Mock
        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(100001)]
    public async Task CreateBuyOrder_BuyOrderQuantityIsMoreThanMaximum_ToBeArgumentException(uint buyOrderQuantity)
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = _fixture.Build<BuyOrderRequest>()
            .With(temp => temp.Quantity, buyOrderQuantity)
            .Create();

        // Mock
        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    public async Task CreateBuyOrder_BuyOrderPriceIsLessThanMinimum_ToBeArgumentException(double buyOrderPrice)
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = _fixture.Build<BuyOrderRequest>()
            .With(temp => temp.Price, buyOrderPrice)
            .Create();

        // Mock
        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(10001)]
    public async Task CreateBuyOrder_BuyOrderPriceIsMoreThanMaximum_ToBeArgumentException(double buyOrderPrice)
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = _fixture.Build<BuyOrderRequest>()
            .With(temp => temp.Price, buyOrderPrice)
            .Create();

        // Mock
        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateBuyOrder_BuyOrderStockSymbolIsNull_ToBeArgumentException()
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = _fixture.Build<BuyOrderRequest>()
            .With(temp => temp.StockSymbol, null as string)
            .Create();

        // Mock
        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateBuyOrder_BuyOrderDateAndTimeOfOrderOlderThan2000_ToBeArgumentexception()
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = _fixture.Build<BuyOrderRequest>()
            .With(temp => temp.DateAndTimeOfOrder, Convert.ToDateTime("1999-12-31"))
            .Create();

        // Mock
        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateBuyOrder_BuyOrderValidData_ToBeSuccessful()
    {
        // Arrange
        BuyOrderRequest? buyOrderRequest = _fixture.Build<BuyOrderRequest>()
            .Create();

        // Mock
        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateBuyOrder(It.IsAny<BuyOrder>()))
            .ReturnsAsync(buyOrder);

        // Act
        BuyOrderResponse buyOrderResponseFromCreate = await _stocksBuyOrdersService.CreateBuyOrder(buyOrderRequest);

        // Assert
        buyOrder.BuyOrderID = buyOrderResponseFromCreate.BuyOrderID;
        BuyOrderResponse buyOrderResponseExpected = buyOrder.ToBuyOrderResponse();
        buyOrderResponseFromCreate.BuyOrderID.Should().NotBe(Guid.Empty);
        buyOrderResponseFromCreate.Should().Be(buyOrderResponseExpected);
    }

    #endregion

    #region CreateSellOrder

    [Fact]
    public async Task CreateSellOrder_NullSellOrder_ToBeArgumentNullException()
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = null;

        // Mock
        SellOrder sellOrder = _fixture.Build<SellOrder>().Create();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0)]
    public async Task CreateSellOrder_SellOrderQuantityIsLessThanMinimum_ToBeArgumentException(uint sellOrderQuantity)
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = _fixture.Build<SellOrderRequest>()
            .With(temp => temp.Quantity, sellOrderQuantity)
            .Create();

        // Mock
        SellOrder sellOrder = sellOrderRequest.ToSellOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(100001)]
    public async Task CreateSellOrder_SellOrderQuantityIsMoreThanMaximum_ToBeArgumentException(uint sellOrderQuantity)
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = _fixture.Build<SellOrderRequest>()
            .With(temp => temp.Quantity, sellOrderQuantity)
            .Create();

        // Mock
        SellOrder sellOrder = sellOrderRequest.ToSellOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    public async Task CreateSellOrder_SellOrderPriceIsLessThanMinimum_ToBeArgumentException(double sellOrderPrice)
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = _fixture.Build<SellOrderRequest>()
            .With(temp => temp.Price, sellOrderPrice)
            .Create();

        // Mock
        SellOrder sellOrder = sellOrderRequest.ToSellOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData(10001)]
    public async Task CreateSellOrder_SellOrderPriceIsMoreThanMaximum_ToBeArgumentException(double sellOrderPrice)
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = _fixture.Build<SellOrderRequest>()
            .With(temp => temp.Price, sellOrderPrice)
            .Create();

        // Mock
        SellOrder sellOrder = sellOrderRequest.ToSellOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateSellOrder_SellOrderStockSymbolIsNull_ToBeArgumentException()
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = _fixture.Build<SellOrderRequest>()
            .With(temp => temp.StockSymbol, null as string)
            .Create();

        // Mock
        SellOrder sellOrder = sellOrderRequest.ToSellOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateSellOrder_SellOrderDateAndTimeOfOrderOlderThan2000_ToBeArgumentException()
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = _fixture.Build<SellOrderRequest>()
            .With(temp => temp.DateAndTimeOfOrder, Convert.ToDateTime("1999-12-31"))
            .Create();

        // Mock
        SellOrder sellOrder = sellOrderRequest.ToSellOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        Func<Task> action = async() =>
        {
            await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);
        };

        // Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateSellOrder_SellOrderValidData_ToBeSuccessful()
    {
        // Arrange
        SellOrderRequest? sellOrderRequest = _fixture.Build<SellOrderRequest>().Create();

        // Mock
        SellOrder sellOrder = sellOrderRequest.ToSellOrder();
        _stocksRepositoryMock.Setup(temp => temp.CreateSellOrder(It.IsAny<SellOrder>()))
            .ReturnsAsync(sellOrder);

        // Act
        SellOrderResponse sellOrderResponseFromCreate = await _stocksSellOrdersService.CreateSellOrder(sellOrderRequest);

        // Assert
        sellOrder.SellOrderID = sellOrderResponseFromCreate.SellOrderID;
        SellOrderResponse sellOrderResponseExpected = sellOrder.ToSellOrderResponse();
        sellOrderResponseFromCreate.SellOrderID.Should().NotBe(Guid.Empty);
        sellOrderResponseFromCreate.Should().Be(sellOrderResponseExpected);
    }

    #endregion

    #region GetBuyOrders

    [Fact]
    public async Task GetBuyOrders_DefaultList_ToBeEmpty()
    {
        // Arrange
        List<BuyOrder> buyOrders = new();

        // Mock
        _stocksRepositoryMock.Setup(temp => temp.GetBuyOrders())
            .ReturnsAsync(buyOrders);

        // Act
        List<BuyOrderResponse> buyOrdersFromGet = await _stocksBuyOrdersService.GetBuyOrders();

        // Assert
        Assert.Empty(buyOrdersFromGet);
    }

    [Fact]
    public async Task GetBuyOrders_WithFewBuyOrders_ToBeSuccessful()
    {
        // Arrange
        List<BuyOrder> buyOrderRequests = new()
        {
            _fixture.Build<BuyOrder>().Create(),
            _fixture.Build<BuyOrder>().Create()
        };

        List<BuyOrderResponse> buyOrdersListExpected = buyOrderRequests
            .Select(temp => temp.ToBuyOrderResponse())
            .ToList();
        List<BuyOrderResponse> buyOrderResponseListFromAdd = new();

        // Mock
        _stocksRepositoryMock.Setup(temp => temp.GetBuyOrders())
            .ReturnsAsync(buyOrderRequests);

        // Act
        List<BuyOrderResponse> buyOrdersListFromGet = await _stocksBuyOrdersService.GetBuyOrders();

        // Assert
        buyOrdersListFromGet.Should().BeEquivalentTo(buyOrdersListExpected);
    }

    #endregion

    #region GetSellOrders

    [Fact]
    public async Task GetSellOrders_DefaultList_ToBeEmpty()
    {
        // Arrange
        List<SellOrder> sellOrders = new();

        // Mock
        _stocksRepositoryMock.Setup(temp => temp.GetSellOrders())
            .ReturnsAsync(sellOrders);

        // Act
        List<SellOrderResponse> sellOrdersFromGet = await _stocksSellOrdersService.GetSellOrders();

        // Assert
        Assert.Empty(sellOrdersFromGet);
    }

    [Fact]
    public async Task GetSellOrders_WithFewSellOrders_ToBeSuccessful()
    {
        // Arrange
        List<SellOrder> sellOrderRequests = new()
        {
            _fixture.Build<SellOrder>().Create(),
            _fixture.Build<SellOrder>().Create()
        };

        List<SellOrderResponse> sellOrdersListExpected = sellOrderRequests
            .Select(temp => temp.ToSellOrderResponse())
            .ToList();
        List<SellOrderResponse> sellOrderResponseListFromAdd = new();

        // Mock
        _stocksRepositoryMock.Setup(temp => temp.GetSellOrders())
            .ReturnsAsync(sellOrderRequests);

        // Act
        List<SellOrderResponse> sellOrdersListFromGet = await _stocksSellOrdersService.GetSellOrders();

        // Assert
        sellOrdersListFromGet.Should().BeEquivalentTo(sellOrdersListExpected);
    }

    #endregion
}