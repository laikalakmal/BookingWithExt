using Core.Domain.Entities;
using Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityConfigurations
{
    // HolidayPackage configuration
    internal class HolidayPackageConfiguration : IEntityTypeConfiguration<HolidayPackage>
    {
        public void Configure(EntityTypeBuilder<HolidayPackage> entity)
        {
            entity.ToTable("HolidayPackages");

            entity.Property(e => e.Property)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.RoomOptions)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Inclusions)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.SpecialOffers)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.CancellationPolicy)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.Images)
                .HasJsonConversion()
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.LastUpdated)
                .HasColumnType("datetime2");
        }
    }

}
