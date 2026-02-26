using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO;

/// <summary>
/// Represents a sell order to sell the stocks
/// </summary>
public class SellOrder
{
    [Key]
    public Guid SellOrderID { get; set; }

    [Required(ErrorMessage = "Stock symbol can't be null or empty")]
    public string StockSymbol { get; set; }

    [Required(ErrorMessage = "Stock name can't be null or empty")]
    public string StockName { get; set; }

    public DateTime DateAndTimeOfOrder { get; set; }

    [Range(1, 100000, ErrorMessage = "You can sell maximum of 100000 stocks in single order, minimum is 1")]
    public uint Quantity { get; set; }

    [Range(1, 10000, ErrorMessage = "The maximum price of stock is 10000, minimum is 1")]
    public double Price { get; set; }
}
