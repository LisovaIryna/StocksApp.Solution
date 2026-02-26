using RepositoryContracts;
using ServiceContracts.StocksService;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services.StocksService;

public class StocksSellOrdersService : ISellOrdersService
{
    private readonly IStocksRepository _stocksRepository;

    public StocksSellOrdersService(IStocksRepository stocksRepository)
    {
        _stocksRepository = stocksRepository;
    }

    public async Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest)
    {
        if (sellOrderRequest == null)
            throw new ArgumentNullException(nameof(sellOrderRequest));

        ValidationHelper.ModelValidation(sellOrderRequest);

        SellOrder sellOrder = sellOrderRequest.ToSellOrder();

        sellOrder.SellOrderID = Guid.NewGuid();

        SellOrder sellOrderRepository = await _stocksRepository.CreateSellOrder(sellOrder);

        return sellOrder.ToSellOrderResponse();
    }

    public async Task<List<SellOrderResponse>> GetSellOrders()
    {
        return (await _stocksRepository.GetSellOrders())
            .Select(temp => temp.ToSellOrderResponse()).ToList();
    }
}
