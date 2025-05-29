using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Controllers;

[Authorize(Roles="Mechanik")]
public class ServiceTaskController : Controller
{
    private readonly UsersDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ServiceTaskController(UserManager<ApplicationUser> userManager, UsersDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }
    
    // GET: /ServiceOrder/AddTask?orderId=5
    [HttpGet]
    public IActionResult Add(int orderId)
    {
        var vm = new ServiceTaskCreateViewModel { ServiceOrderId = orderId };
        return View(vm);  // Views/ServiceOrder/AddTask.cshtml
    }

// POST: /ServiceOrder/AddTask
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(ServiceTaskCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var task = new ServiceTask
        {
            ServiceOrderId = vm.ServiceOrderId,
            Description    = vm.Description,
            LaborCost      = vm.LaborCost
        };
        _context.ServiceTasks.Add(task);
        await _context.SaveChangesAsync();

        // po dodaniu wróć do szczegółów zlecenia
        //return RedirectToAction("Details", new { id = vm.ServiceOrderId });
        return RedirectToAction("Details", "ServiceOrder",
            new { id = vm.ServiceOrderId });
    }
}