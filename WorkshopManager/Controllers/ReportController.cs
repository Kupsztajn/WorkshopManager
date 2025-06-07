using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Controllers;

public class ReportController : Controller
{
    
    private readonly UsersDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public ReportController(UserManager<ApplicationUser> userManager, UsersDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet]
    public IActionResult Report(string clientId, int? month, int? vehicleId)
    {
        var orders = _context.ServiceOrders
            .Include(o => o.Vehicle)
            .Include(o => o.ServiceTasks)
                .ThenInclude(st => st.UsedParts)
                    .ThenInclude(up => up.Part)
            .Where(o => o.Vehicle.ClientId == clientId);

        if (month != null)
        {
            orders = orders.Where(o => o.CreatedAt.Month == month);
        }

        if (vehicleId != null)
        {
            orders = orders.Where(o => o.VehicleId == vehicleId);
        }

        List<ReportItem> reportItems = orders.ToList().Select(o => new ReportItem
        {
            Date = o.CreatedAt,
            Vehicle = o.Vehicle,
            LaborCost = o.ServiceTasks.Sum(s => s.LaborCost),
            PartsCost = o.ServiceTasks.Sum(st => st.UsedParts.Sum(up => up.TotalCost))
        }).ToList();


        ApplicationUser? client = _context.Users.FirstOrDefault(u => u.Id == clientId);
        if (client is null)
        {
            return Problem(title: $"Client id {clientId} not found");
        }

        var vm = new ReportViewModel()
        {
            Client = client,
            ReportItems = reportItems,
            TotalCost = reportItems.Sum(s => s.LaborCost + s.PartsCost),
            SelectedMonth = month,
            SelectedVehicleId = vehicleId,
            Vehicles = _context.Vehicles.Where(v => v.ClientId == clientId).ToList()
        };

        return View(vm);
    }
}