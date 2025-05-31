using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Controllers;

[Authorize(Roles = "Mechanik")]
public class ServiceTaskPartController : Controller
{
    private readonly UsersDbContext _context;

    public ServiceTaskPartController(UsersDbContext context)
    {
        _context = context;
    }

    // GET: /ServiceTaskPart/Add?serviceTaskId=123
    [HttpGet]
    public async Task<IActionResult> Add(int serviceTaskId)
    {
        // Sprawdź, czy istnieje takie zadanie
        var task = await _context.ServiceTasks
            .Include(st => st.ServiceOrder)
            .FirstOrDefaultAsync(st => st.Id == serviceTaskId);

        if (task == null)
            return NotFound();

        // Przygotuj ViewModel z ID zadania
        var vm = new UsedPartCreateViewModel
        {
            ServiceTaskId = serviceTaskId,
            PartId = 0,       // domyślnie puste
            Quantity = 1
        };

        // Pobierz listę części do dropdowna
        var parts = await _context.Parts
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.PartsList = parts
            .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} (Cena: {p.UnitPrice:F2} PLN)"
            })
            .ToList();

        // Przekaż nazwę zadania i zlecenia do widoku (opcjonalnie)
        ViewBag.TaskDescription = task.Description;
        ViewBag.OrderId = task.ServiceOrderId;
        return View(vm);  // szuka Views/ServiceTaskPart/Add.cshtml
    }

    // POST: /ServiceTaskPart/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(UsedPartCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            // W razie błędów musisz ponownie załadować PartsList
            var parts = await _context.Parts.OrderBy(p => p.Name).ToListAsync();
            ViewBag.PartsList = parts
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} (Cena: {p.UnitPrice:F2} PLN)"
                })
                .ToList();

            // Aby wrócić do widoku z zachowaniem ID zadania
            ViewBag.TaskDescription = (await _context.ServiceTasks.FindAsync(vm.ServiceTaskId))?.Description;
            ViewBag.OrderId = (await _context.ServiceTasks.FindAsync(vm.ServiceTaskId))?.ServiceOrderId;
            return View(vm);
        }

        // Pobierz encję ServiceTask (z ServiceOrderId)
        var task = await _context.ServiceTasks
            .Include(st => st.ServiceOrder)
            .FirstOrDefaultAsync(st => st.Id == vm.ServiceTaskId);

        if (task == null)
            return NotFound();

        // Pobierz wybraną część, żeby odczytać UnitPrice
        var part = await _context.Parts.FindAsync(vm.PartId);
        if (part == null)
        {
            ModelState.AddModelError("PartId", "Wybrana część nie istnieje");
            // odśwież listę części
            var partsAll = await _context.Parts.OrderBy(p => p.Name).ToListAsync();
            ViewBag.PartsList = partsAll
                .Select(p2 => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p2.Id.ToString(),
                    Text = $"{p2.Name} (Cena: {p2.UnitPrice:F2} PLN)"
                })
                .ToList();

            ViewBag.TaskDescription = task.Description;
            ViewBag.OrderId = task.ServiceOrderId;
            return View(vm);
        }

        // Oblicz koszt części: unit price * quantity
        decimal totalCost = part.UnitPrice * vm.Quantity;

        // Utwórz rekord UsedPart
        var usedPart = new UsedPart
        {
            ServiceTaskId = vm.ServiceTaskId,
            PartId = vm.PartId,
            Quantity = vm.Quantity,
            TotalCost = totalCost
        };

        _context.UsedParts.Add(usedPart);
        await _context.SaveChangesAsync();

        // Po dodaniu części: przekieruj z powrotem do szczegółów zlecenia (dla tego Mechanika lub Recepcjonisty)
        // Tutaj zakładam, że mechanik chce zostać w swoich „MyOrders”,
        // ale zwykle chcemy od razu wrócić do widoku Details zlecenia, bo tam widać wszystkie czynności i części.
        // Pobierz ID zlecenia:
        int orderId = task.ServiceOrderId;

        // Jeśli mechanik dodaje, prawdopodobnie chce nadal widzieć szczegóły zlecenia:
        return RedirectToAction("Details", "ServiceOrder", new { id = orderId });
    }
}