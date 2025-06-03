using Core.Domain.Entities;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Persistence.EntityConfigurations
{
    // TourPackage configuration
    internal class TourPackageConfiguration : IEntityTypeConfiguration<TourPackage>
    {
        public void Configure(EntityTypeBuilder<TourPackage> entity)
        {
            entity.ToTable("TourPackages");

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

}