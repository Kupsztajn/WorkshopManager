using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Data;
using WorkshopManager.Models;
using WorkshopManager.Services;

namespace WorkshopManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Services.AddDbContext<UsersDbContext>(options => options.UseSqlite("Data Source=database.db"));
            
            builder.Services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer("Server=localhost;Database=DbWorkshop;Trusted_Connection=True;TrustServerCertificate=True;"));
            
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => { options.LoginPath = "/Account/Login"; });
            builder.Services.AddScoped<IUserService, UserService>();
            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
