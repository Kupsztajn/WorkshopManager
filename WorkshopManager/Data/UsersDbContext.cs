using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Models;

namespace WorkshopManager.Data
{
    public class UsersDbContext : IdentityDbContext<ApplicationUser>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

        // Możesz dodać dodatkowe DbSet<>, np. Klienci, Pojazdy itp.
        //public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}