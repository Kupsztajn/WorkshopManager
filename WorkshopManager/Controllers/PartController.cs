using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels;

namespace WorkshopManager.Controllers;

[Authorize(Roles="Admin,Recepcjonista")]
public class PartController : Controller
{
    private readonly UsersDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PartController(UserManager<ApplicationUser> userManager, UsersDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet]
    public IActionResult Add()
    {
        var vm = new PartCreateViewModel();
        return View(vm);  
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(PartCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var part = new Part
        {
            Name = vm.Name,
            UnitPrice = vm.UnitPrice
        };
        _context.Parts.Add(part);
        await _context.SaveChangesAsync();
        
        return RedirectToAction("List", "Part");
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        return View(_context.Parts.ToList());
    }
    
    public IActionResult Edit(int id)
    {
        var part = _context.Parts.Find(id);
        if (part == null)
            return NotFound();

        return View(part);
    }

    [HttpPost]
    public IActionResult Edit(int id, Part part)
    {
        if (id != part.Id || !ModelState.IsValid)
            return View(part);

        _context.Update(part);
        _context.SaveChanges();
        return RedirectToAction("List");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var part = _context.Parts.Find(id);
        if (part == null)
            return NotFound();

        _context.Parts.Remove(part);
        _context.SaveChanges();
        return RedirectToAction("List");
    }
}