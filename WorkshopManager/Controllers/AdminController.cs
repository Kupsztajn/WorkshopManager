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
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }


        // GET: /Admin/EditUserRoles/{userId}
        public async Task<IActionResult> EditUserRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = allRoles.Select(role => new RoleSelection
                {
                    RoleName = role,
                    Selected = userRoles.Contains(role)
                }).ToList()
            };

            return View(model);
        }
        // POST: /Admin/EditUserRoles
        [HttpPost]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            ModelState.Remove("Email");
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            var rolesToAdd = model.Roles.Where(r => r.Selected && !userRoles.Contains(r.RoleName)).Select(r => r.RoleName);
            var rolesToRemove = userRoles.Where(r => !model.Roles.Any(mr => mr.RoleName == r && mr.Selected));

            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!addResult.Succeeded || !removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Nie udało się zmienić ról użytkownika");
                return View(model);
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