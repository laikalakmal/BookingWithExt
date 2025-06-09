using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.EntityConfigurations
{
    // Product configuration
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

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

            entity.Property(e => e.Attributes)
               .HasColumnName("Attributes")
               .HasColumnType("nvarchar(max)")
               .IsRequired(false)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, _jsonOptions),
                   v => DeserializeAttributes(v)
               );

            entity.Property(e => e.ImageUrl)
                .HasColumnName("ImageUrl")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => DeserializeImageUrls(v)
                );
        }

        private Dictionary<string, object> DeserializeAttributes(string v)
        {
            if (string.IsNullOrEmpty(v))
                return new Dictionary<string, object>();

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(v, _jsonOptions) 
                    ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        private List<string> DeserializeImageUrls(string v)
        {
            if (string.IsNullOrEmpty(v))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) 
                    ?? [];
            }
            catch
            {
                if (v != null && v.StartsWith("http"))
                {
                    return [v];
                }
                return [];
            }
        }
    }
}