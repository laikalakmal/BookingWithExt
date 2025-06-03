using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.EntityConfigurations
{
    internal class CustomProductConfiguration : IEntityTypeConfiguration<CustomProduct>
    {
        public void Configure(EntityTypeBuilder<CustomProduct> entity)
        {
            entity.ToTable("CustomProducts");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            entity.Property(e => e.Attributes)
                .HasColumnName("Attributes")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, options),
                    v => string.IsNullOrEmpty(v)
                        ? new Dictionary<string, object>()
                        : JsonSerializer.Deserialize<Dictionary<string, object>>(v, options) ?? new Dictionary<string, object>() // Fix for CS8603
                );

            entity.Property(e => e.ImageUrl)
                .HasColumnName("ImageUrl")
                .HasColumnType("nvarchar(255)")
                .IsRequired(false);
        }
    }
}
