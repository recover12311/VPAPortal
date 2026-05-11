using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using VPAPortal.Data.Models;

namespace VPAPortal.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Crew> Crews { get; set; }
        public DbSet<DroneItem> DroneItems { get; set; }
        public DbSet<AmmoItem> AmmoItems { get; set; }
        public DbSet<WarehouseDroneItem> WarehouseDroneItems { get; set; }
        public DbSet<WarehouseAmmoItem> WarehouseAmmoItems { get; set; }
        public DbSet<WarehouseInvoice> WarehouseInvoices { get; set; }
        public DbSet<WarehouseLog> WarehouseLogs { get; set; }
        public DbSet<CrewLog> CrewLogs { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<FlightDrop> FlightDrops { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<PropertyItem> PropertyItems { get; set; }
        public DbSet<PropertyLog> PropertyLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Flight>()
                .HasOne(f => f.Crew)
                .WithMany(c => c.Flights)
                .HasForeignKey(f => f.CrewId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Flight>()
                .HasOne(f => f.DroneItem)
                .WithMany()
                .HasForeignKey(f => f.DroneItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FlightDrop>()
                .HasOne(d => d.AmmoItem)
                .WithMany()
                .HasForeignKey(d => d.AmmoItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FlightDrop>()
                .HasOne(d => d.AmmoItem)
                .WithMany()
                .HasForeignKey(d => d.AmmoItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Crew>()
                .HasOne(c => c.Company)
                .WithMany(co => co.Crews)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CrewLog>()
                .HasOne(l => l.Crew)
                .WithMany(c => c.CrewLogs)
                .HasForeignKey(l => l.CrewId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Flight>()
                .HasOne(f => f.DroneItem)
                .WithMany()
                .HasForeignKey(f => f.DroneItemId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PropertyType>().HasData(
                new PropertyType { Id = 1, Name = "Радіолокаційне", SortOrder = 1 },
                new PropertyType { Id = 2, Name = "Автомобільна техніка", SortOrder = 2 },
                new PropertyType { Id = 3, Name = "Засоби зв'язку", SortOrder = 3 },
                new PropertyType { Id = 4, Name = "Стрілецька зброя", SortOrder = 4 },
                new PropertyType { Id = 5, Name = "Боєприпаси", SortOrder = 5 },
                new PropertyType { Id = 6, Name = "Спорядження", SortOrder = 6 },
                new PropertyType { Id = 7, Name = "Інше", SortOrder = 7 }
);
        }
    }
}