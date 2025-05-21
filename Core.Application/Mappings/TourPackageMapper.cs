using Core.Application.DTOs;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;

namespace Core.Application.Mappings
{
    public class TourPackageMapper : IProductMapper<TourPackage, TourPackageDto>
    {
        public static TourPackageDto FromDomain(Product product)
        {
            if (product is not TourPackage tourPackage)
                throw new ArgumentException("Product must be of type TourPackage", nameof(product));

            return new TourPackageDto(
                id: tourPackage.Id,
                externalId: tourPackage.ExternalId,
                name: tourPackage.Name,
                price: tourPackage.Price,
                description: tourPackage.Description ?? string.Empty,
                category: tourPackage.Category,
                provider: tourPackage.Provider,
                imageUrl: tourPackage.Images?.FirstOrDefault() ?? string.Empty,
                createdAt: tourPackage.CreatedAt,
                updatedAt: tourPackage.UpdatedAt,
                destination: tourPackage.Destination is null
                    ? new TourPackageDto.DestinationInfo { Country = string.Empty, City = string.Empty, Resort = string.Empty }
                    : new TourPackageDto.DestinationInfo
                    {
                        Country = tourPackage.Destination.Country ?? string.Empty,
                        City = tourPackage.Destination.City ?? string.Empty,
                        Resort = tourPackage.Destination.Resort ?? string.Empty
                    },
                duration: tourPackage.Duration is null
                    ? new TourPackageDto.DurationInfo()
                    : new TourPackageDto.DurationInfo
                    {
                        Days = tourPackage.Duration.Days,
                        Nights = tourPackage.Duration.Nights
                    },
                inclusions: tourPackage.Inclusions ?? new List<string>(),
                exclusions: tourPackage.Exclusions ?? new List<string>(),
                departureDates: tourPackage.DepartureDates ?? new List<DateTime>(),
                accommodation: tourPackage.Accommodation is null
                    ? new TourPackageDto.AccommodationInfo { Type = string.Empty, Rating = 0, Amenities = new List<string>() }
                    : new TourPackageDto.AccommodationInfo
                    {
                        Type = tourPackage.Accommodation.Type ?? string.Empty,
                        Rating = tourPackage.Accommodation.Rating,
                        Amenities = tourPackage.Accommodation.Amenities ?? new List<string>()
                    },
                transportation: tourPackage.Transportation is null
                    ? new TourPackageDto.TransportationInfo()
                    : new TourPackageDto.TransportationInfo
                    {
                        Flight = tourPackage.Transportation.Flight is null ? null : new TourPackageDto.FlightInfo
                        {
                            Airline = tourPackage.Transportation.Flight.Airline ?? string.Empty,
                            Class = tourPackage.Transportation.Flight.Class ?? string.Empty,
                            Included = tourPackage.Transportation.Flight.Included
                        },
                        Transfers = tourPackage.Transportation.Transfers is null ? null : new TourPackageDto.TransferInfo
                        {
                            Type = tourPackage.Transportation.Transfers.Type ?? string.Empty,
                            Included = tourPackage.Transportation.Transfers.Included
                        }
                    },
                cancellationPolicy: tourPackage.CancellationPolicy is null
                    ? new CancellationPolicyInfo { FreeCancellation = false, Deadline = string.Empty, Penalty = string.Empty }
                    : new CancellationPolicyInfo
                    {
                        FreeCancellation = tourPackage.CancellationPolicy.FreeCancellation,
                        Deadline = tourPackage.CancellationPolicy.Deadline ?? string.Empty,
                        Penalty = tourPackage.CancellationPolicy.Penalty ?? string.Empty
                    },
                availability: tourPackage.Availability is null
                    ? new AvailabilityInfo { Status = string.Empty, RemainingSlots = 0 }
                    : new AvailabilityInfo
                    {
                        Status = tourPackage.Availability.Status ?? string.Empty,
                        RemainingSlots = tourPackage.Availability.RemainingSlots
                    },
                images: tourPackage.Images ?? new List<string>(),
                termsAndConditions: tourPackage.TermsAndConditions ?? string.Empty,
                lastUpdated: tourPackage.LastUpdated
            );
        }

        public static TourPackage ToDomain(ProductDto productDto)
        {
            if (productDto is not TourPackageDto tourPackageDto)
                throw new ArgumentException("ProductDto must be of type TourPackageDto", nameof(productDto));

            var destination = tourPackageDto.Destination is null
                ? new DestinationInfo()
                : new DestinationInfo
                {
                    Country = tourPackageDto.Destination.Country,
                    City = tourPackageDto.Destination.City,
                    Resort = tourPackageDto.Destination.Resort
                };

            var duration = tourPackageDto.Duration is null
                ? new DurationInfo()
                : new DurationInfo
                {
                    Days = tourPackageDto.Duration.Days,
                    Nights = tourPackageDto.Duration.Nights
                };

            var accommodation = tourPackageDto.Accommodation is null
                ? new AccommodationInfo()
                : new AccommodationInfo
                {
                    Type = tourPackageDto.Accommodation.Type,
                    Rating = tourPackageDto.Accommodation.Rating,
                    Amenities = tourPackageDto.Accommodation.Amenities ?? new List<string>()
                };

            var transportation = tourPackageDto.Transportation is null
                ? new TransportationInfo()
                : new TransportationInfo
                {
                    Flight = tourPackageDto.Transportation.Flight is null ? null : new FlightInfo
                    {
                        Airline = tourPackageDto.Transportation.Flight.Airline,
                        Class = tourPackageDto.Transportation.Flight.Class,
                        Included = tourPackageDto.Transportation.Flight.Included
                    },
                    Transfers = tourPackageDto.Transportation.Transfers is null ? null : new TransferInfo
                    {
                        Type = tourPackageDto.Transportation.Transfers.Type,
                        Included = tourPackageDto.Transportation.Transfers.Included
                    }
                };

            var cancellationPolicy = tourPackageDto.CancellationPolicy is null
                ? new CancellationPolicyInfo()
                : new CancellationPolicyInfo
                {
                    FreeCancellation = tourPackageDto.CancellationPolicy.FreeCancellation,
                    Deadline = tourPackageDto.CancellationPolicy.Deadline ?? string.Empty,
                    Penalty = tourPackageDto.CancellationPolicy.Penalty ?? string.Empty
                };

            var availability = tourPackageDto.Availability is null
                ? new AvailabilityInfo()
                : new AvailabilityInfo
                {
                    Status = tourPackageDto.Availability.Status,
                    RemainingSlots = tourPackageDto.Availability.RemainingSlots
                };

            var tourPackage = new TourPackage(
                externalId: tourPackageDto.ExternalId,
                provider: tourPackageDto.Provider,
                name: tourPackageDto.Name,
                price: tourPackageDto.Price,
                description: tourPackageDto.Description,
                destination: destination,
                duration: duration,
                inclusions: tourPackageDto.Inclusions ?? [],
                exclusions: tourPackageDto.Exclusions ?? [],
                departureDates: tourPackageDto.DepartureDates ?? [],
                accommodation: accommodation,
                transportation: transportation,
                cancellationPolicy: cancellationPolicy,
                availability: availability,
                images: tourPackageDto.Images ?? [],
                termsAndConditions: tourPackageDto.TermsAndConditions ?? string.Empty,
                lastUpdated: tourPackageDto.LastUpdated
            )
            {
                Id = tourPackageDto.Id,
                CreatedAt = tourPackageDto.CreatedAt,
                UpdatedAt = tourPackageDto.UpdatedAt,
                Destination = destination,
                Duration = duration,
                Inclusions = tourPackageDto.Inclusions ?? [],
                Exclusions = tourPackageDto.Exclusions ?? [],
                DepartureDates = tourPackageDto.DepartureDates ?? new List<DateTime>(),
                Accommodation = accommodation,
                Transportation = transportation,
                CancellationPolicy = cancellationPolicy,
                Availability = availability,
                Images = tourPackageDto.Images ?? [],
                TermsAndConditions = tourPackageDto.TermsAndConditions ?? string.Empty,
                LastUpdated = tourPackageDto.LastUpdated
            };

            return tourPackage;
        }
    }
}
