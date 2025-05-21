using Core.Domain.Entities;
using Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new TourPackageConfiguration());
            modelBuilder.ApplyConfiguration(new HolidayPackageConfiguration());
        }
    }

}

