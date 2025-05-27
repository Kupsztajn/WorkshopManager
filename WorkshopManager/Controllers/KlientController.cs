using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;

namespace WorkshopManager.Controllers;

public class KlientController : Controller
{
    private readonly UsersDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public KlientController(UsersDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Recepcjonista
    public async Task<IActionResult> Index()
    {
        // User.Identity.Name to domyślnie UserName/email
        var username = User.Identity.Name;
        var user = await _userManager.FindByNameAsync(username);
        
        var roles = await _userManager.GetRolesAsync(user);
        Console.WriteLine(string.Join(", ", roles));
            
        // Przekazujemy imię do widoku np. przez ViewBag
        ViewBag.FirstName = user?.Name;
        
        
            
        return View();
    }
    
    public async Task<IActionResult> MyVehicles()
    {
        var user = await _userManager.GetUserAsync(User);

        var vehicles = await _context.Vehicles
            .Where(v => v.ClientId == user.Id)
            .ToListAsync();

        return View(vehicles);
    }
}