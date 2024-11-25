using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

public class LegacyHomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}