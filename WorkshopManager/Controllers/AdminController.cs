using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
                return View(model);
                // lub po prostu View(model)
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
        
        // podobnie dodasz AddMechanic
        [HttpGet]
        public IActionResult AddMechanic()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddMechanic(CreateUserViewModel model)
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
                await _userManager.AddToRoleAsync(user, "Mechanik");
                TempData["SuccessMessage"] = "Mechanik został poprawnie dodany.";
                return RedirectToAction(nameof(AddReceptionist)); // lub inna akcja
            }
            else
            {
                TempData["ErrorMessage"] = "Nie udało się dodać Mechanik: " + string.Join(", ", result.Errors.Select(e => e.Description));
                return View(model);
                // lub po prostu View(model)
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }
        
        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();

            var usersWithRoles = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Surname = user.Surname,
                    Roles = roles.ToList()  // lista ról użytkownika
                });
            }

            return View(usersWithRoles);
        }

        
        // GET: /Admin/EditUserRoles/{userId}
        public async Task<IActionResult> EditUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                SelectedRole = userRoles.FirstOrDefault(),
                AllRoles = allRoles
            };

            return View(model);
        }
        // POST: /Admin/EditUserRoles
        [HttpPost]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            ModelState.Remove("Email");
            ModelState.Remove("AllRoles");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                var result = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Nie udało się przypisać roli.");
                    return View(model);
                }
            }
            return RedirectToAction(nameof(Users));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}