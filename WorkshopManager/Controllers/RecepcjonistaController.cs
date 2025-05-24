using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Data;
using WorkshopManager.Models;

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
    public async Task<IActionResult> AddClient(Client model)
    {
        ModelState.Remove("AddedByUserId");
        ModelState.Remove("AddedByUser");

        if (!ModelState.IsValid)
        {
            // Zbierz wszystkie komunikaty błędów
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            // Przekaż je do widoku przez ViewBag
            ViewBag.Errors = errors;
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        model.AddedByUserId = user.Id;
        
        _context.Clients.Add(model);
        await _context.SaveChangesAsync();

        // Po dodaniu klienta możesz przekierować do listy klientów lub panelu
        return RedirectToAction("Index");
    }
}