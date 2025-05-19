using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<TourPackage> TourPackages { get; set; }
        public DbSet<HolidayPackage> HolidayPackages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.ToTable("products");
            });
            modelBuilder.Entity<TourPackage>().ToTable("TourPackages");
            modelBuilder.Entity<HolidayPackage>().ToTable("HolidayPackages");

            // Seed dummy data for TourPackage
            modelBuilder.Entity<TourPackage>().HasData(
                new TourPackage(
                    "TP001", "City Tour", 99.99m, "A fun city tour", "Tour", "tourApi", "1 day", "citytour.jpg")
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed dummy data for HolidayPackage
            modelBuilder.Entity<HolidayPackage>().HasData(
                new HolidayPackage(
                    "HP001", "Beach Holiday", 499.99m, "Relaxing beach holiday", "Holiday", "HolidayApi", "7 days", "beachholiday.jpg")
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
