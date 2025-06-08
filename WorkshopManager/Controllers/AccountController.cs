using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkshopManager.Models;
using WorkshopManager.Models.ViewModels; // LoginViewModel

namespace WorkshopManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Tutaj przekierowujesz po zalogowaniu
                return RedirectToAction("RedirectAfterLogin");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser 
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account.");
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                await _userManager.AddToRoleAsync(user, model.Role);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("RedirectAfterLogin");
            }
            else
            {
                _logger.LogInformation("User failed to create a new account.");
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model); // Pokaż widok z błędami
                
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
            //return RedirectToAction("Index", "Home");
        }
    
        [Authorize]
        public async Task<IActionResult> RedirectAfterLogin()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "Admin");

            if (roles.Contains("Mechanik"))
                return RedirectToAction("Index", "Mechanik");

            if (roles.Contains("Recepcjonista"))
                return RedirectToAction("Index", "Recepcjonista");

            if (roles.Contains("Klient"))
                return RedirectToAction("Index", "Klient");

            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            return View();
        }

    }
}