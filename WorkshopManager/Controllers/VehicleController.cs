using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Data;
using WorkshopManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using WorkshopManager.Models.ViewModels;

[Authorize(Roles = "Recepcjonista,Admin,Klient")]  // lub kto może dodawać pojazdy
public class VehicleController : Controller
{
    private readonly UsersDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;
    public VehicleController(UsersDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    // GET: Vehicle/Add/{clientId}
    [HttpGet]
    public IActionResult Add(string clientId)
    {
        var model = new VehicleUploadViewModel { ClientId = clientId };
        return View(model);
    }

    /*
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
    */
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(VehicleUploadViewModel vm)
    {
        ModelState.Remove("Photo");
        ModelState.Remove("Client");
        ModelState.Remove("ImageUrl");
        if (!ModelState.IsValid) return View(vm);

        // 1) zapisz plik na dysku
        if (vm.Photo != null && vm.Photo.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            // nadaj unikalną nazwę
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.Photo.FileName)}";
            var filePath = Path.Combine(uploads, fileName);

            using var stream = System.IO.File.Create(filePath);
            await vm.Photo.CopyToAsync(stream);

            // 2) utwórz Vehicle i ustaw ImageUrl
            var vehicle = new Vehicle {
                Brand = vm.Brand,
                Model = vm.Model,
                VIN = vm.VIN,
                Registration = vm.Registration,
                Year =  vm.Year,
                ClientId = vm.ClientId,
                ImageUrl = $"/uploads/{fileName}"
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Recepcjonista");
        }

        ModelState.AddModelError("Photo", "Musisz wybrać plik.");
        return View(vm);
    }

    
    // GET: /Vehicle/Details/5
    //[Authorize(Roles = "Klient,Recepcjonista,Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        
        // Debug - sprawdź wartości
        Console.WriteLine($"Vehicle.ClientId: '{vehicle.ClientId}'");
        Console.WriteLine($"User.Id: '{user.Id}'");
        Console.WriteLine($"User.IsInRole Klient: {User.IsInRole("Klient")}");
        
        // Sprawdź czy klient jest właścicielem pojazdu lub ma rolę Recepcjonista/Admin
        if (User.IsInRole("Klient") && vehicle.ClientId != user.Id)
        {
            return Forbid(); // lub RedirectToAction("AccessDenied", "Account");
        }

        return View(vehicle);
    }
}