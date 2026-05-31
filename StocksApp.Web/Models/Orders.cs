using ServiceContracts.DTO;

namespace StocksApp.Models;

/// <summary>
/// Represents model class to supply list of buy orders and sell orders to the Trades/Orders view
/// </summary>
public class Orders
{
    public List<BuyOrderResponse> BuyOrders { get; set; } = new();
    public List<SellOrderResponse> SellOrders { get; set; } = new();
}
