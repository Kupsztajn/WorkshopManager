using Microsoft.AspNetCore.Authorization;
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
        if (User.Identity.IsAuthenticated == false)
        {
            return Unauthorized();
        }
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
    [HttpGet]
    public IActionResult Comments(int id)
    {
        if (User.Identity.IsAuthenticated == false)
        {
            return Unauthorized();
        }
        var order = _context.ServiceOrders
            .Include(o => o.Comments) // assuming there's a navigation property
            .FirstOrDefault(o => o.Id == id);

        if (order == null) return NotFound();

        return View(order); // This will look for Views/ServiceOrder/Comments.cshtml
    }

    [HttpPost]
    public IActionResult Comments(int serviceOrderId, string content)
    {
        if (User.Identity.IsAuthenticated == false)
        {
            return Unauthorized();
        }
        Comment comment = new Comment();
        comment.Content = content;
        comment.Author = User.Identity.Name;
        comment.Timestamp = DateTime.Now;
        ServiceOrder serviceOrder = _context.ServiceOrders.Find(serviceOrderId);
        if (serviceOrder == null) return NotFound();
        serviceOrder.Comments.Add(comment);
        _context.SaveChanges();
        return RedirectToAction("Comments", new { id = serviceOrderId });
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
    
        // GET: /ServiceOrder/EditStatus/5
    // Wyświetla formularz do zmiany statusu
    [Authorize]  // najpierw weźmy auth, potem sprawdzimy w kodzie, czy ma prawo
    [HttpGet]
    public async Task<IActionResult> EditStatus(int id)
    {
        // 1) Pobierz zlecenie wraz z Mechanikiem (do weryfikacji uprawnień)
        var order = await _context.ServiceOrders
            .Include(o => o.Mechanic)
            .Include(o => o.Vehicle)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return NotFound();

        // 2) Pobierz bieżącego usera
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge(); // nie jest zalogowany

        // 3) Sprawdź uprawnienia: 
        //    - jeśli nie jest w roli Admin i nie jest przypisanym mechanikiem, to forbidd
        bool isAdmin = User.IsInRole("Admin");
        bool isAssignedMechanic = (order.MechanicId == user.Id);

        if (!isAdmin && !isAssignedMechanic)
            return Forbid();

        // 4) Przygotuj ViewModel
        var vm = new ServiceOrderStatusEditViewModel
        {
            OrderId = order.Id,
            CurrentStatus = order.Status,
            // Dostępne statusy – zwykle pomijamy ten sam, co Current, ale można go zostawić
            StatusesList = new List<string> { "Nowe", "W trakcie", "Zakończone", "Anulowane" }
        };

        return View(vm); // Views/ServiceOrder/EditStatus.cshtml
    }

    // POST: /ServiceOrder/EditStatus
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStatus(ServiceOrderStatusEditViewModel vm)
    {
        ModelState.Remove("StatusesList");
        if (!ModelState.IsValid)
        {
            // W razie invalidu ponownie ustaw listę statusów i zwróć widok
            vm.StatusesList = new List<string> { "Nowe", "W trakcie", "Zakończone", "Anulowane" };
            return View(vm);
        }

        // 1) Pobierz zlecenie (razem z mechanic i vehicle)
        var order = await _context.ServiceOrders
            .Include(o => o.Mechanic)
            .Include(o => o.Vehicle)
            .FirstOrDefaultAsync(o => o.Id == vm.OrderId);

        if (order == null)
            return NotFound();

        // 2) Weryfikacja uprawnień – jak wyżej w GET
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        bool isAdmin = User.IsInRole("Admin");
        bool isAssignedMechanic = (order.MechanicId == user.Id);

        if (!isAdmin && !isAssignedMechanic)
            return Forbid();

        // 3) Zmiana statusu
        order.Status = vm.NewStatus;

        // 4) Jeśli nowy status to "Zakończone", ustaw CompletedAt
        if (vm.NewStatus == "Zakończone")
        {
            order.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            // Jeśli ktoś zmienił status np. z "Zakończone" na inny, wyczyść completed date
            order.CompletedAt = null;
        }

        // 5) Zapisz zmiany
        _context.ServiceOrders.Update(order);
        await _context.SaveChangesAsync();

        // 6) Przekierowanie – jeżeli zmienił mechanik, możesz wysłać do własnych zleceń,
        //    ale najczęściej wracamy do szczegółów zlecenia:
        return RedirectToAction("Details", new { id = order.Id });
    }
}