using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels; // jeśli masz CreateUserViewModel

namespace WorkshopManager.Controllers
{
    [Authorize(Roles = "Admin")] // tylko admin może korzystać z tego kontrolera
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult AddReceptionist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReceptionist(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                Name = model.Name,
                Surname = model.Surname,
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Recepcjonista");
                TempData["SuccessMessage"] = "Użytkownik został poprawnie dodany.";
                return RedirectToAction(nameof(AddReceptionist)); // lub inna akcja
            }
            else
            {
                TempData["ErrorMessage"] = "Nie udało się dodać użytkownika: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return RedirectToAction("AddReceptionist", model); // lub po prostu View(model)
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
        
        // podobnie dodasz AddMechanic
    }
}