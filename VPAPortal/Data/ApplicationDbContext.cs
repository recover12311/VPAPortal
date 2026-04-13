using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VPAPortal.Data.Models;

namespace VPAPortal.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
    {
        public DbSet<Crew> Crews { get; set; }
        public DbSet<DroneItem> DroneItems { get; set; }
        public DbSet<AmmoItem> AmmoItems { get; set; }
        public DbSet<Flight> Flights { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Flight -> Crew: без каскаду
            builder.Entity<Flight>()
                .HasOne(f => f.Crew)
                .WithMany(c => c.Flights)
                .HasForeignKey(f => f.CrewId)
                .OnDelete(DeleteBehavior.NoAction);

            // Flight -> DroneItem: без каскаду
            builder.Entity<Flight>()
                .HasOne(f => f.DroneItem)
                .WithMany()
                .HasForeignKey(f => f.DroneItemId)
                .OnDelete(DeleteBehavior.NoAction);

            // Flight -> AmmoItem: без каскаду
            builder.Entity<Flight>()
                .HasOne(f => f.AmmoItem)
                .WithMany()
                .HasForeignKey(f => f.AmmoItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}