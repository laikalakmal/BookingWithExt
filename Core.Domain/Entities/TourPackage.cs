using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Domain.Entities
{
    public class TourPackage : Product
    {
        public TourPackage(
            string externalId,
            string provider,
            string name,
            Price price,
            string description,
            DestinationInfo destination,
            DurationInfo duration,
            List<string> inclusions,
            List<string> exclusions,
            List<DateTime> departureDates,
            AccommodationInfo accommodation,
            TransportationInfo transportation,
            CancellationPolicyInfo cancellationPolicy,
            AvailabilityInfo availability,
            List<string> images,
            string termsAndConditions,
            DateTime lastUpdated
        ) : base(
                externalId: externalId,
                 name:name,
                 price:price,
                 availability: availability,
                 description:description,
                 category: ProductCategory.TourPackage,
                provider: provider)
        {
            Destination = destination;
            Duration = duration;
            Inclusions = inclusions;
            Exclusions = exclusions;
            DepartureDates = departureDates;
            Accommodation = accommodation;
            Transportation = transportation;
            CancellationPolicy = cancellationPolicy;
            Images = images;
            TermsAndConditions = termsAndConditions;
            LastUpdated = lastUpdated;
        }

        public TourPackage() // Parameterless constructor for EF Core
            : base(string.Empty, string.Empty, Price.Create(0, "USD"), string.Empty, ProductCategory.TourPackage, "",new AvailabilityInfo("",0))
        {

            Inclusions = new List<string>();
            Exclusions = new List<string>();
            DepartureDates = new List<DateTime>();
            Images = new List<string>();
            ExternalId = string.Empty;
            Destination = new DestinationInfo();
            Duration = new DurationInfo();
            Accommodation = new AccommodationInfo();
            Transportation = new TransportationInfo();
            CancellationPolicy = new CancellationPolicyInfo();
            TermsAndConditions = string.Empty;
            LastUpdated = DateTime.UtcNow;


        }


        public DestinationInfo Destination { get; set; }
        public DurationInfo Duration { get; set; }
        public List<string> Inclusions { get; set; }
        public List<string> Exclusions { get; set; }
        public List<DateTime> DepartureDates { get; set; }
        public AccommodationInfo Accommodation { get; set; }
        public TransportationInfo Transportation { get; set; }
        public CancellationPolicyInfo CancellationPolicy { get; set; }
        public List<string> Images { get; set; }
        public string TermsAndConditions { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class DestinationInfo
    {
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Resort { get; set; }
    }

    public class DurationInfo
    {
        public int Days { get; set; }
        public int Nights { get; set; }
    }

    public class AccommodationInfo
    {
        public string? Type { get; set; }
        public int Rating { get; set; }
        public List<string>? Amenities { get; set; }
    }

    public class TransportationInfo
    {
        public FlightInfo? Flight { get; set; }
        public TransferInfo? Transfers { get; set; }
    }

    public class FlightInfo
    {
        public string? Airline { get; set; }
        public string? Class { get; set; }
        public bool Included { get; set; }
    }

    public class TransferInfo
    {
        public string? Type { get; set; }
        public bool Included { get; set; }
    }
}
