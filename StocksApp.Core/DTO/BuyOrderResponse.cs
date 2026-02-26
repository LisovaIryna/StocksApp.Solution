namespace ServiceContracts.DTO;

/// <summary>
/// DTO class that represents a buy order to purchase the stocks
/// </summary>
public class BuyOrderResponse : IOrderResponse
{
    public Guid BuyOrderID { get; set; }
    public string StockSymbol { get; set; }
    public string StockName { get; set; }
    public DateTime DateAndTimeOfOrder { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }
    public OrderType TypeOfOrder => OrderType.BuyOrder;
    public double TradeAmount { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not BuyOrderResponse)
            return false;

        BuyOrderResponse other = (BuyOrderResponse)obj;

        bool isEqual = BuyOrderID == other.BuyOrderID && StockSymbol == other.StockSymbol && StockName == other.StockName
            && DateAndTimeOfOrder == other.DateAndTimeOfOrder && Quantity == other.Quantity && Price == other.Price;
        return isEqual;
    }

    public override int GetHashCode()
    {
        return StockSymbol.GetHashCode();
    }

    public override string ToString()
    {
        return $"Buy Order ID: {BuyOrderID}, Stock symbol: {StockSymbol}, Stock name: {StockName}, " +
            $"Date and time of buy order: {DateAndTimeOfOrder.ToString("dd MMM yyyy hh:mm ss tt")}, " +
            $"Quantity: {Quantity}, Buy price: {Price}, Trade amount: {TradeAmount}";
    }
}

public static class BuyOrderExtensions
{
    public static BuyOrderResponse ToBuyOrderResponse(this BuyOrder buyOrder)
    {
        return new BuyOrderResponse()
        {
            BuyOrderID = buyOrder.BuyOrderID,
            StockSymbol = buyOrder.StockSymbol,
            StockName = buyOrder.StockName,
            DateAndTimeOfOrder = buyOrder.DateAndTimeOfOrder,
            Quantity = buyOrder.Quantity,
            Price = buyOrder.Price,
            TradeAmount = buyOrder.Price * buyOrder.Quantity
        };
    }
}
