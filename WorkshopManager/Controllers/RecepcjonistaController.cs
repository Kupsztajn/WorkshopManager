using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Models;

namespace WorkshopManager.Controllers;

[Authorize(Roles = "Recepcjonista")]
public class RecepcjonistaController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RecepcjonistaController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: /Recepcjonista
    public async Task<IActionResult> Index()
    {
        // User.Identity.Name to domyślnie UserName/email
        var username = User.Identity.Name;
        var user = await _userManager.FindByNameAsync(username);
            
        // Przekazujemy imię do widoku np. przez ViewBag
        ViewBag.FirstName = user?.Name;
            
        return View();
    }
}