using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;
using WorkshopManager.Pdf;

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

        if (!(User.IsInRole("Admin") || User.IsInRole("Recepcjonista")))
        {
            return Unauthorized();
        }
        
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

    private MonthlyReportViewModel GetMonthlyReportViewModel(int month, int year)
    {
        List<MonthlyReportItem> reportItems = _context.ServiceOrders
            .Include(o => o.Vehicle.Client)
            .Include(o => o.Vehicle)
            .Include(o => o.ServiceTasks)
            .ThenInclude(st => st.UsedParts)
            .ThenInclude(up => up.Part)
            .Where(o => o.CreatedAt.Month == month && o.CreatedAt.Year == year)
            .ToList()
            .GroupBy(o => new { o.Vehicle.Client, o.Vehicle })
            .Select(
                g => new MonthlyReportItem()
                {
                    Client = g.Key.Client,
                    Vehicle = g.Key.Vehicle,
                    TotalCost = g.Sum(o => o.ServiceTasks.Sum(st => st.LaborCost + st.UsedParts
                            .Sum(up => up.TotalCost)
                        )
                    ),
                    OrderCount = g.Count()
                }
            ).ToList();
        
        MonthlyReportViewModel vm = new MonthlyReportViewModel()
        {
            Month = month,
            Year = year,
            ReportItems = reportItems,
            TotalCost = reportItems.Sum(s => s.TotalCost)
        };

        return vm;
    }
    
    [HttpGet]
    public IActionResult MonthlyReport(int? month, int? year)
    {
        
        if (!User.IsInRole("Admin"))
        {
            return Unauthorized();
        }

        if (month is null)
        {
            month = DateTime.Now.Month;
        }

        if (year is null)
        {
            year = DateTime.Now.Year;
        }
        
        MonthlyReportViewModel vm = GetMonthlyReportViewModel(month.Value, year.Value);

        return View(vm);
    }

    public IActionResult GemerateMonthlyReportPdf(int month, int year)
    {
        MonthlyReportViewModel vm = GetMonthlyReportViewModel(month, year);

        MonthlyReportDocument doc = new MonthlyReportDocument(vm);
        QuestPDF.Settings.License = LicenseType.Community;
        byte[] pdfBytes = doc.GeneratePdf();
        
        return File(pdfBytes, "application/pdf", $"{month}-{year}.pdf");
    }
}