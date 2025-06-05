using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services;
using WorkshopManager.Services.Implementations;
using WorkshopManager.Services.Interfaces;
namespace WorkshopManager
{
    public class Program
    {
        public static async Task SeedUsersAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Utwórz role jeśli nie istnieją
            string[] roles = new[] { "Admin", "Mechanik", "Recepcjonista", "Klient"};
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Utwórz użytkownika admin jeśli nie istnieje
            var adminEmail = "admin@warsztat.pl";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin",
                    Surname = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");  // Hasło jawne - zostanie zahashowane

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Tutaj możesz logować błędy tworzenia użytkownika
                    throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Services.AddDbContext<UsersDbContext>(options => options.UseSqlite("Data Source=database.db"));

            string connectionString;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                connectionString =
                    "Server=localhost;Database=DbWorkshop;Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                connectionString =
                    "Server=localhost,1433;Database=DbWorkshop;User Id=SA;Password=YourStrong_Passw0rd;TrustServerCertificate=True;";
            }
            else
            {
                throw new Exception("Platform not supported");
            }
            
            
            
            builder.Services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer(connectionString, sqlserverOptions => sqlserverOptions.EnableRetryOnFailure()));
            
            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //.AddCookie(options => { options.LoginPath = "/Account/Login"; });
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<UsersDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });
            
            builder.Services.AddScoped<IMechanicService, MechanicService>();
            
            var app = builder.Build();
            
            await SeedUsersAsync(app);

            
            using (var scope = app.Services.CreateScope())
            {
                var usersContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
                usersContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
