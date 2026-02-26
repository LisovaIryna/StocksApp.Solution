using ServiceContracts.DTO;

namespace RepositoryContracts;

/// <summary>
/// Represents Stock service
/// </summary>
public interface IStocksRepository
{
    Task<BuyOrder> CreateBuyOrder(BuyOrder buyOrder);

    Task<SellOrder> CreateSellOrder(SellOrder sellOrder);

    Task<List<BuyOrder>> GetBuyOrders();

    Task<List<SellOrder>> GetSellOrders();
}
