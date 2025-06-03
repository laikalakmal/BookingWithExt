using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations
{
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

            entity.Property(p => p.Category)
                .HasConversion<string>()
                .HasColumnType("nvarchar(50)");

            entity.OwnsOne(p => p.Availability, availability =>
            {
                availability.Property(a => a.IsAvailable).HasColumnType("bit");
                availability.Property(a => a.Status).HasColumnType("nvarchar(max)");
                availability.Property(a => a.RemainingSlots).HasConversion<int>();
            });


        }
    }

}