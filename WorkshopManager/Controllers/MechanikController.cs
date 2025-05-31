using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;

namespace WorkshopManager.Controllers;

public class MechanikController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UsersDbContext _context;

    public MechanikController(UsersDbContext context, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _context = context;
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
    public async Task<IActionResult> MyOrders(string mechanicId = null)
    {
        if (string.IsNullOrEmpty(mechanicId))
        {
            // Jeśli nie podano mechanicId, to spróbuj pobrać Id zalogowanego użytkownika (mechanika)
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge(); // nie zalogowany

            mechanicId = user.Id;
        }

        var orders = await _context.ServiceOrders
            .Where(o => o.MechanicId == mechanicId)
            .Include(o => o.Vehicle)
            .ToListAsync();

        return View(orders);
    }
    
    // GET: /Mechanik/MyOrders
    /*
    public async Task<IActionResult> MyOrders()
    {
        var mech = await _userManager.GetUserAsync(User);
        var orders = await _context.ServiceOrders
            .Where(o => o.MechanicId == mech.Id)
            .Include(o => o.Vehicle)
            .ToListAsync();

        return View(orders);
    }
    */
    
}