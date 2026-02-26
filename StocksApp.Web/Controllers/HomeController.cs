using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StocksApp.Models;

namespace StocksApp.Controllers;

public class HomeController : Controller
{
    [Route("Error")]
    public IActionResult Error()
    {
        IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
        {
            Error error = new()
            {
                ErrorMessage = exceptionHandlerPathFeature.Error.Message
            };
            return View(error);
        }
        else
        {
            Error error = new()
            {
                ErrorMessage = "Error encountered"
            };
            return View(error);
        }
    }
}
