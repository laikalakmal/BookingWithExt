using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Application.DTOs
{
    public class HolidayPackageDto : ProductDto
    {
        public PropertyInfo Property { get; set; }
        public List<RoomOption> RoomOptions { get; set; }
        public List<string> Inclusions { get; set; }
        public List<SpecialOffer> SpecialOffers { get; set; }
        public CancellationPolicyInfo CancellationPolicy { get; set; }
        public List<string> Images { get; set; }
        public DateTime LastUpdated { get; set; }

        public HolidayPackageDto(
            Guid id,
            string externalId,
            string name,
            Price price,
            string description,
            ProductCategory category,
            string provider,
            string imageUrl,
            DateTime createdAt,
            DateTime updatedAt,
            PropertyInfo property,
            List<RoomOption> roomOptions,
            List<string> inclusions,
            List<SpecialOffer> specialOffers,
            CancellationPolicyInfo cancellationPolicy,
            List<string> images,
            DateTime lastUpdated) : base(
                id,
                externalId,
                name,
                price,
                description,
                category,
                provider,
                imageUrl,
                createdAt,
                updatedAt)
        {
            Property = property;
            RoomOptions = roomOptions;
            Inclusions = inclusions;
            SpecialOffers = specialOffers;
            CancellationPolicy = cancellationPolicy;
            Images = images;
            LastUpdated = lastUpdated;
        }

        public class PropertyInfo
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public double Rating { get; set; }
            public string? Type { get; set; }
            public LocationInfo? Location { get; set; }
            public List<string>? Amenities { get; set; }
        }

        public class LocationInfo
        {
            public string? Country { get; set; }
            public string? Island { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? SkiArea { get; set; }
            public string? Neighborhood { get; set; }
            public string? Reserve { get; set; }
            public string? NearestTown { get; set; }
            public string? Lagoon { get; set; }
            public List<string>? Landmarks { get; set; }
            public Coordinates? Coordinates { get; set; }
            public string? TransferTime { get; set; }
        }

        public class Coordinates
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public class RoomOption
        {
            public string? RoomType { get; set; }
            public int MaxOccupancy { get; set; }
            public string? Size { get; set; }
            public string? BedType { get; set; }
            public string? View { get; set; }
            public List<string>? Amenities { get; set; }
            public RoomPrice? Price { get; set; }
        }

        public class RoomPrice
        {
            public decimal PerNight { get; set; }
            public decimal Total { get; set; }
            public string? Currency { get; set; }
            public string? MealPlan { get; set; }
        }

        public class SpecialOffer
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public decimal? Discount { get; set; }
            public DateTime? ValidUntil { get; set; }
            public bool RequiresVerification { get; set; }
            public decimal? PriceAddon { get; set; }
            public string? Currency { get; set; }
            public string? Code { get; set; }
            public int? MinimumNights { get; set; }
            public List<string>? ValidFor { get; set; }
        }


    }
}
