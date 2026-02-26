namespace ServiceContracts.DTO;

/// <summary>
/// DTO class that represents a sell order
/// </summary>
public class SellOrderResponse : IOrderResponse
{
    public Guid SellOrderID { get; set; }
    public string StockSymbol { get; set; }
    public string StockName { get; set; }
    public DateTime DateAndTimeOfOrder { get; set; }
    public uint Quantity { get; set; }
    public double Price { get; set; }
    public OrderType TypeOfOrder => OrderType.SellOrder;
    public double TradeAmount { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not SellOrderResponse)
            return false;

        SellOrderResponse other = (SellOrderResponse)obj;

        bool isEqual = SellOrderID == other.SellOrderID && StockSymbol == other.StockSymbol && StockName == other.StockName
            && DateAndTimeOfOrder == other.DateAndTimeOfOrder && Quantity == other.Quantity && Price == other.Price;
        return isEqual;
    }

    public override int GetHashCode()
    {
        return StockSymbol.GetHashCode();
    }

    public override string ToString()
    {
        return $"Sell Order ID: {SellOrderID}, Stock symbol: {StockSymbol}, Stock name: {StockName}, " +
            $"Date and time of sell order: {DateAndTimeOfOrder.ToString("dd MMM yyyy hh:mm ss tt")}, " +
            $"Quantity: {Quantity}, Sell price: {Price}, Trade Amount: {TradeAmount}";
    }
}

public static class SellOrderExtensions
{
    public static SellOrderResponse ToSellOrderResponse(this SellOrder sellOrder)
    {
        return new SellOrderResponse()
        {
            SellOrderID = sellOrder.SellOrderID,
            StockSymbol = sellOrder.StockSymbol,
            StockName = sellOrder.StockName,
            DateAndTimeOfOrder = sellOrder.DateAndTimeOfOrder,
            Quantity = sellOrder.Quantity,
            Price = sellOrder.Price,
            TradeAmount = sellOrder.Price * sellOrder.Quantity
        };
    }
}
