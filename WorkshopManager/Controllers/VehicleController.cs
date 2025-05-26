using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Data;
using WorkshopManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Recepcjonista,Admin")]  // lub kto może dodawać pojazdy
public class VehicleController : Controller
{
    private readonly UsersDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public VehicleController(UsersDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Vehicle/Add/{clientId}
    [HttpGet]
    public IActionResult Add(string clientId)
    {
        var model = new Vehicle { ClientId = clientId };
        return View(model);
    }

    // POST: Vehicle/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Vehicle vmodel)
    {
        
        // Usuń błędy walidacji dla pól, które ustawiasz ręcznie lub nie chcesz walidować
        // ModelState.Remove("Brand");
        // ModelState.Remove("Model");
        // ModelState.Remove("VIN");
        // ModelState.Remove("Registration");
        // ModelState.Remove("Year");
        // ModelState.Remove("ClientId");
        ModelState.Remove("Client");

        // Teraz wypisz wszystkie błędy walidacji (jeśli są jakieś inne)
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        if (errors.Any())
        {
            foreach (var error in errors)
            {
                Console.WriteLine("Błąd walidacji: " + error);
            }
        }
        
        if (!ModelState.IsValid)
            return View(vmodel);

        // Zapisz pojazd z przypisanym ClientId
        _context.Vehicles.Add(vmodel);
        await _context.SaveChangesAsync();

        // Przekieruj 
        return RedirectToAction("Index", "Recepcjonista");
    }
}