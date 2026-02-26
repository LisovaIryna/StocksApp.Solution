namespace StocksApp.Models;

/// <summary>
/// Model class to supply trade details
/// </summary>
public class StockTrade
{
    public string? StockSymbol { get; set; }
    public string? StockName { get; set; }
    public double Price { get; set; }
    public uint Quantity { get; set; }
}
