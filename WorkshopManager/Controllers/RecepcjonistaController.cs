using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WorkshopManager.Controllers;

[Authorize(Roles = "Recepcjonista")]
public class RecepcjonistaController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}