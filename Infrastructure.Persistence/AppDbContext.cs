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

        public DbSet<CustomProduct> CustomProducts { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // when you add a new entity, you need to add it here as well to be able to use it in the database.
        // Before adding a new entity, make sure to create a configuration class for it in the EntityConfigurations folder.(lakmal@5/22/2025)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new TourPackageConfiguration());
            modelBuilder.ApplyConfiguration(new HolidayPackageConfiguration());
            modelBuilder.ApplyConfiguration(new CustomProductConfiguration());
        }
    }

}

