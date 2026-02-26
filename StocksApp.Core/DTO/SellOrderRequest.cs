using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO;

/// <summary>
/// DTO class that represents a sell order
/// </summary>
public class SellOrderRequest : IValidatableObject, IOrderRequest
{
    [Required(ErrorMessage = "Stock symbol can't be null or empty")]
    public string StockSymbol { get; set; }

    [Required(ErrorMessage = "Stock name can't be null or empty")]
    public string StockName { get; set; }

    public DateTime DateAndTimeOfOrder { get; set; }

    [Range(1, 100000, ErrorMessage = "You can sell maximum of 100000 stocks in single order, minimum is 1")]
    public uint Quantity { get; set; }

    [Range(1, 10000, ErrorMessage = "The maximum price of stock is 10000, minimum is 1")]
    public double Price { get; set; }

    public SellOrder ToSellOrder()
    {
        return new SellOrder()
        {
            StockSymbol = StockSymbol,
            StockName = StockName,
            Price = Price,
            DateAndTimeOfOrder = DateAndTimeOfOrder,
            Quantity = Quantity
        };
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> results = new();

        if (DateAndTimeOfOrder < Convert.ToDateTime("2000-01-01"))
            results.Add(new ValidationResult("Order date should not be older than Jan 01, 2000"));

        return results;
    }
}
