using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
}