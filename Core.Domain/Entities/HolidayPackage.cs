using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Domain.Entities
{
    public partial class HolidayPackage : Product
    {
        public HolidayPackage(
            string externalId,
            string provider,
            AvailabilityInfo availability,
            string name,
            Price price,
            string description,
            PropertyInfo property,
            List<RoomOption> roomOptions,
            List<string> inclusions,
            List<SpecialOffer> specialOffers,
            CancellationPolicyInfo cancellationPolicy,
            List<string> images,
            DateTime lastUpdated) : base(
                externalId,
                name,
                price,
                description,
                ProductCategory.HolidayPackage,
                provider,
                availability
                )
        {
            Property = property;
            RoomOptions = roomOptions;
            Inclusions = inclusions;
            SpecialOffers = specialOffers;
            CancellationPolicy = cancellationPolicy;
            Images = images;
            LastUpdated = lastUpdated;
        }

        public HolidayPackage() // Parameterless constructor for EF Core
            : base(string.Empty, string.Empty, Price.Create(0, "USD"), string.Empty, ProductCategory.HolidayPackage, "booking.com",availability: new AvailabilityInfo("",0))
        {
            Property = new PropertyInfo();
            RoomOptions = new List<RoomOption>();
            Inclusions = new List<string>();
            SpecialOffers = new List<SpecialOffer>();
            CancellationPolicy = new CancellationPolicyInfo();
            Images = new List<string>();
            LastUpdated = DateTime.UtcNow;
        }

        public PropertyInfo Property { get; set; }
        public List<RoomOption> RoomOptions { get; set; }
        public List<string> Inclusions { get; set; }
        public List<SpecialOffer> SpecialOffers { get; set; }
        public CancellationPolicyInfo CancellationPolicy { get; set; }
        public List<string> Images { get; set; }
        public DateTime LastUpdated { get; set; }

        public class PropertyInfo
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public double Rating { get; set; }
            public string Type { get; set; } = string.Empty;
            public LocationInfo Location { get; set; } = new LocationInfo();
            public List<string> Amenities { get; set; } = new List<string>();
        }

        public class LocationInfo
        {
            public string Country { get; set; } = string.Empty;
            public string Island { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
            public string SkiArea { get; set; } = string.Empty;
            public string Neighborhood { get; set; } = string.Empty;
            public string Reserve { get; set; } = string.Empty;
            public string NearestTown { get; set; } = string.Empty;
            public string Lagoon { get; set; } = string.Empty;
            public List<string> Landmarks { get; set; } = new List<string>();
            public Coordinates Coordinates { get; set; } = new Coordinates();
            public string TransferTime { get; set; } = string.Empty;
        }

        public class RoomOption
        {
            public string RoomType { get; set; } = string.Empty;
            public int MaxOccupancy { get; set; }
            public string Size { get; set; } = string.Empty;
            public string BedType { get; set; } = string.Empty;
            public string View { get; set; } = string.Empty;
            public List<string> Amenities { get; set; } = new List<string>();
            public RoomPrice Price { get; set; } = new RoomPrice();
        }

        public class RoomPrice
        {
            public decimal PerNight { get; set; }
            public decimal Total { get; set; }
            public string Currency { get; set; } = string.Empty;
            public string MealPlan { get; set; } = string.Empty;
        }

    }
}
