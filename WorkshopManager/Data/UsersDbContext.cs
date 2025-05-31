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
        //public DbSet<Client> Clients { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<ServiceTask> ServiceTasks { get; set; }
        public DbSet<UsedPart> UsedParts { get; set; }
        public DbSet<Part> Parts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // relacja ServiceOrder → Vehicle bez kaskady
            builder.Entity<ServiceOrder>()
                .HasOne(o => o.Vehicle)
                .WithMany(v => v.ServiceOrders)     // jeśli masz kolekcję w Vehicle
                .HasForeignKey(o => o.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // relacja ServiceOrder → Mechanic (AspNetUsers) – tu można zostawić kaskadę
            builder.Entity<ServiceOrder>()
                .HasOne(o => o.Mechanic)
                .WithMany()                         // bez kolekcji w ApplicationUser
                .HasForeignKey(o => o.MechanicId)
                .OnDelete(DeleteBehavior.Cascade);

            // domyślny status
            builder.Entity<ServiceOrder>()
                .Property(o => o.Status)
                .HasDefaultValue("Nowe");
            
            builder.Entity<ServiceTask>()
                .HasOne(t => t.ServiceOrder)
                .WithMany(o => o.ServiceTasks)
                .HasForeignKey(t => t.ServiceOrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Relacja 1:N ServiceTask → UsedPart
            builder.Entity<UsedPart>()
                .HasOne(up => up.ServiceTask)
                .WithMany(st => st.UsedParts)           // ← dopisz w ServiceTask kolekcję UsedParts
                .HasForeignKey(up => up.ServiceTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacja 1:N Part → UsedPart (opcjonalnie, jeżeli potrzebujesz nawigacji w Part)
            builder.Entity<UsedPart>()
                .HasOne(up => up.Part)
                .WithMany()                              // zakładamy, że nie masz kolekcji w Part
                .HasForeignKey(up => up.PartId)
                .OnDelete(DeleteBehavior.Restrict);
            
            
        }
    }
    
    
}