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
    
    // GET: /Klient/OrdersForVehicle?vehicleId=123
    // Wyświetla listę zleceń przypisanych do danego pojazdu (tylko klient- właściciel pojazdu)
    public async Task<IActionResult> OrdersForVehicle(int vehicleId)
    {
        // 1) Odczyt użytkownika i zabezpieczenie
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        // 2) Sprawdzamy, czy taki pojazd w ogóle istnieje i należy do tego klienta
        var vehicle = await _context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId && v.ClientId == user.Id);
        if (vehicle == null)
            return NotFound(); 

        // 3) Wczytujemy zlecenia dla tego pojazdu, łącznie z podstawowymi danymi (np. status, data, mechanik, liczba czynności)
        var orders = await _context.ServiceOrders
            .Where(o => o.VehicleId == vehicleId)
            .Include(o => o.Mechanic)
            // jeśli chcemy wyświetlać liczbę czynności:
            .Include(o => o.ServiceTasks)
            .ToListAsync();

        ViewBag.VehicleName = $"{vehicle.Brand} {vehicle.Model} (VIN: {vehicle.VIN})";
        ViewBag.VehicleId = vehicleId;
        return View(orders);
    }

    // GET: /Klient/OrderDetails/{orderId}
    // Szczegóły zlecenia (lista czynności + części) – tylko do odczytu
    public async Task<IActionResult> OrderDetails(int orderId)
    {
        // 1) Pobranie użytkownika
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        // 2) Wczytujemy konkretne zlecenie z wszystkimi nawigacjami:
        var order = await _context.ServiceOrders
            .Include(o => o.Vehicle)
            .Include(o => o.Mechanic)
            .Include(o => o.ServiceTasks)
                .ThenInclude(st => st.UsedParts)
                    .ThenInclude(up => up.Part)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return NotFound();

        // 3) Sprawdźmy, czy zlecenie należy do jednego z pojazdów tego klienta
        if (order.Vehicle.ClientId != user.Id)
            return Forbid();

        ViewBag.VehicleId = order.VehicleId;
        return View(order);
    }
}