using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using StocksApp.Controllers;
using StocksApp.Models;

namespace StocksApp.Filters.ActionFilters;

/// <summary>
/// An action filter that applies model validation to SellOrder() and BuyOrder() 
/// </summary>
public class CreateOrderActionFilter : IAsyncActionFilter
{
    public CreateOrderActionFilter() { }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.Controller is TradeController tradeController)
        {
            var orderRequest = context.ActionArguments["orderRequest"] as IOrderRequest;

            if (orderRequest != null)
            {
                // update date of order
                orderRequest.DateAndTimeOfOrder = DateTime.Now;

                // re-validate the model object aqfter updating the date
                tradeController.ModelState.Clear();
                tradeController.TryValidateModel(orderRequest);

                if (!tradeController.ModelState.IsValid)
                {
                    tradeController.ViewBag.Errors = tradeController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    StockTrade stockTrade = new()
                    {
                        StockName = orderRequest.StockName,
                        StockSymbol = orderRequest.StockSymbol,
                        Quantity = orderRequest.Quantity
                    };
                    context.Result = tradeController.View(nameof(TradeController.Index), stockTrade);
                }
                else
                    await next();
            }
            else
                await next();
        }
        await next();
    }
}
