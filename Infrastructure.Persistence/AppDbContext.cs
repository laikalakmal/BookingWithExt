using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<TourPackage> TourPackages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new TourPackageConfiguration());
        }
    }

    // Product configuration
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.ToTable("Products");

            entity.OwnsOne(p => p.Price, price =>
            {
                price.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                price.Property(p => p.Currency).HasConversion<string>();
            });
        }
    }

    // TourPackage configuration
    internal class TourPackageConfiguration : IEntityTypeConfiguration<TourPackage>
    {
        public void Configure(EntityTypeBuilder<TourPackage> entity)
        {
            entity.Property(e => e.Destination)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Duration)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Accommodation)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Transportation)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.CancellationPolicy)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Availability)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Inclusions)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Exclusions)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Images)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.TermsAndConditions)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.LastUpdated)
                .HasColumnType("datetime2");

            entity.Property(e => e.DepartureDates)
                .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                  v => JsonSerializer.Deserialize<List<DateTime>>(v, (JsonSerializerOptions?)null) ?? new List<DateTime>())
                .HasColumnType("nvarchar(max)");

        }
    }

    // Helper extension method
    internal static class ValueConversionExtensions
    {
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
        {
            return propertyBuilder.HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<T>(v, (JsonSerializerOptions?)null) ?? default!);
        }
    }
}