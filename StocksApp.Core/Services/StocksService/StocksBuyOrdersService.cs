using RepositoryContracts;
using ServiceContracts.StocksService;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services.StocksService;

public class StocksBuyOrdersService : IBuyOrdersService
{
    private readonly IStocksRepository _stocksRepository;

    public StocksBuyOrdersService(IStocksRepository stocksRepository)
    {
        _stocksRepository = stocksRepository;
    }

    public async Task<BuyOrderResponse> CreateBuyOrder(BuyOrderRequest? buyOrderRequest)
    {
        if (buyOrderRequest == null)
            throw new ArgumentNullException(nameof(buyOrderRequest));

        ValidationHelper.ModelValidation(buyOrderRequest);

        BuyOrder buyOrder = buyOrderRequest.ToBuyOrder();

        buyOrder.BuyOrderID = Guid.NewGuid();

        BuyOrder buyOrderRepository = await _stocksRepository.CreateBuyOrder(buyOrder);

        return buyOrder.ToBuyOrderResponse();
    }

    public async Task<List<BuyOrderResponse>> GetBuyOrders()
    {
        return (await _stocksRepository.GetBuyOrders())
            .Select(temp => temp.ToBuyOrderResponse()).ToList();
    }
}
