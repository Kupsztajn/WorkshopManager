using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Controllers;

public class MechanikController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}