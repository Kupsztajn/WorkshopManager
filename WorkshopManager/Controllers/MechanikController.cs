using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Models;

namespace WorkshopManager.Controllers;

public class MechanikController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public MechanikController(UserManager<ApplicationUser> userManager)
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