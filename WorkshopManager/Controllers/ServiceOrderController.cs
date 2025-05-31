using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Controllers;

public class ServiceOrderController :  Controller
{
    private readonly UsersDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ServiceOrderController(UserManager<ApplicationUser> userManager, UsersDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _context.ServiceOrders
            .Include(o => o.Vehicle)
            .Include(o => o.Mechanic)
            .Include(o => o.ServiceTasks)
            // a do każdej czynności dokładać jej UsedParts
            .ThenInclude(st => st.UsedParts)
            // i dalej: do każdego UsedPart dokładać encję Part (żeby poznać Name i UnitPrice)
            .ThenInclude(up => up.Part)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        return View(order);
    }
    
    public async Task<IActionResult> OrdersForVehicle(int vehicleId)
    {
        var orders = await _context.ServiceOrders
            .Where(o => o.VehicleId == vehicleId)
            .Include(o => o.Mechanic)
            .ToListAsync();

        ViewBag.VehicleId = vehicleId;
        return View(orders);
    }
    
    //GET
    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        var vehicle = await _context.Vehicles.FindAsync(id);
        if (vehicle == null) return NotFound();

        // Pobierz mechaników do dropdowna
        var mechanics = await _userManager.GetUsersInRoleAsync("Mechanik");
        ViewBag.Mechanics = mechanics.Select(m => new SelectListItem 
        { 
            Value = m.Id, 
            Text = m.Name + " " + m.Surname 
        }).ToList();

        // Przekaż do ViewModel VehicleId i może klienta jeśli potrzebujesz
        var vm = new ServiceOrderCreateViewModel
        {
            VehicleId = id,
            Description = "",
            Status = "Nowe"
        };

        ViewBag.VehicleDisplay = $"{vehicle.Brand} {vehicle.Model} ({vehicle.VIN})";

        return View(vm);
    }
    
    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceOrderCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // przy błędach ponownie załaduj listy
            var clients = await _userManager.GetUsersInRoleAsync("Klient");
            ViewBag.Clients = clients.Select(c => new SelectListItem { Value = c.Id, Text = c.Name + " " + c.Surname }).ToList();

            var mechanics = await _userManager.GetUsersInRoleAsync("Mechanik");
            ViewBag.Mechanics = mechanics.Select(m => new SelectListItem { Value = m.Id, Text = m.Name + " " + m.Surname }).ToList();

            return View(model);
        }

        // Pobierz pojazd i mechanika
        var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);
        var mechanic = await _userManager.FindByIdAsync(model.MechanicId);

        if (vehicle == null)
        {
            ModelState.AddModelError("VehicleId", "Wybrany pojazd nie istnieje");
            return View(model);
        }

        // Utwórz zlecenie
        var order = new ServiceOrder
        {
            VehicleId = model.VehicleId,
            Description = model.Description,
            Status = "Nowe",
            MechanicId = model.MechanicId
        };

        _context.ServiceOrders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("OrdersForVehicle", new { vehicleId = model.VehicleId });
    }
    
    

}