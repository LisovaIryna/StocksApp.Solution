using ServiceContracts.DTO;

namespace ServiceContracts.StocksService;

/// <summary>
/// Represents Stocks service
/// </summary>
public interface ISellOrdersService
{
    /// <summary>
    /// Creates a sell order
    /// </summary>
    /// <param name="sellOrderRequest">Sell order object</param>
    Task<SellOrderResponse> CreateSellOrder(SellOrderRequest? sellOrderRequest);

    /// <summary>
    /// Returns all existing sell orders
    /// </summary>
    /// <returns>Returns a list of objects of SellOrder type</returns>
    Task<List<SellOrderResponse>> GetSellOrders();
}
