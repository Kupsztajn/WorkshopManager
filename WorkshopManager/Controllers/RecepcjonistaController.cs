using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Controllers;

[Authorize(Roles = "Recepcjonista")]
public class RecepcjonistaController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UsersDbContext _context;

    public RecepcjonistaController(UserManager<ApplicationUser> userManager, UsersDbContext context)
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
    
    // GET: Recepcjonista/AddClient
    public IActionResult AddClient()
    {
        return View();
    }

    // POST: Recepcjonista/AddClient
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddClient(RegisterViewModel model)
    {
        ModelState.Remove("AddedByUserId");
        ModelState.Remove("AddedByUser");

        if (!ModelState.IsValid)
            return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            Name = model.Name,
            Surname = model.Surname,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Klient");
            return RedirectToAction("Index"); // lub inna akcja
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }
    
    // GET: Recepcjonista/Clients
    public async Task<IActionResult> Clients()
    {
        var clientsIList = await _userManager.GetUsersInRoleAsync("Klient");
        var clients = clientsIList.ToList();  // Konwersja na List<ApplicationUser>
        return View(clients);

    }
    
    
    // GET: /Recepcjonista/ClientVehicles/{clientId}
    public async Task<IActionResult> ClientVehicles(string clientId)
    {
        if (string.IsNullOrEmpty(clientId)) return NotFound();
        var client = await _userManager.FindByIdAsync(clientId);
        if (client == null) return NotFound();

        var vehicles = await _context.Vehicles
            .Where(v => v.ClientId == clientId)
            .ToListAsync();

        ViewBag.ClientName = client.Name + " " + client.Surname;
        return View(vehicles);
    }

}