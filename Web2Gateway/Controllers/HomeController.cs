namespace Web2Gateway;

using Microsoft.AspNetCore.Mvc;


public class HomeController : ControllerBase
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return RedirectToAction("GetWellKnown", "WellKnown");
    }
}
