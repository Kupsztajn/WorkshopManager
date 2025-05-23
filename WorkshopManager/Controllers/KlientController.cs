using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Controllers;

public class KlientController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}