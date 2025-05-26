using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Infrastructure.Adapters
{
    public class TourApiAdapter : IExternalProductApiAdapter
    {
        public Task<ProductDto?> FetchProductByIdAsync(string externalId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ProductDto>> FetchProductsAsync()
        {
            var apiUrl = "https://cc392dcc-ec4b-491a-a6af-c2e1b7ca4750.mock.pstmn.io/tours";
            var apiKey = "your-api-key";
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            // Define DTO for deserialization
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var apiPackages = System.Text.Json.JsonSerializer.Deserialize<List<ApiTourPackage>>(json, options);

            // Map to TourPackageDto
            var result = new List<ProductDto>();
            if (apiPackages != null)
            {
                foreach (var package in apiPackages)
                {
                    var price = Price.Create(
                        package.Price?.Amount ?? 0m,
                        package.Price?.Currency?.ToString() ?? "USD"
                    );

                    // Provide non-null default objects for required parameters
                    var destination = package.Destination is null
                        ? new TourPackageDto.DestinationInfo { Country = string.Empty, City = string.Empty, Resort = string.Empty }
                        : new TourPackageDto.DestinationInfo
                        {
                            Country = package.Destination.Country ?? string.Empty,
                            City = package.Destination.City ?? string.Empty,
                            Resort = package.Destination.Resort ?? string.Empty
                        };

                    var duration = package.Duration is null
                        ? new TourPackageDto.DurationInfo()
                        : new TourPackageDto.DurationInfo
                        {
                            Days = package.Duration.Days,
                            Nights = package.Duration.Nights
                        };

                    var accommodation = package.Accommodation is null
                        ? new TourPackageDto.AccommodationInfo { Type = string.Empty, Rating = 0, Amenities = new List<string>() }
                        : new TourPackageDto.AccommodationInfo
                        {
                            Type = package.Accommodation.Type ?? string.Empty,
                            Rating = package.Accommodation.Rating,
                            Amenities = package.Accommodation.Amenities ?? new List<string>()
                        };

                    var transportation = package.Transportation is null
                        ? new TourPackageDto.TransportationInfo()
                        : new TourPackageDto.TransportationInfo
                        {
                            Flight = package.Transportation.Flight is null ? null : new TourPackageDto.FlightInfo
                            {
                                Airline = package.Transportation.Flight.Airline ?? string.Empty,
                                Class = package.Transportation.Flight.Class ?? string.Empty,
                                Included = package.Transportation.Flight.Included
                            },
                            Transfers = package.Transportation.Transfers is null ? null : new TourPackageDto.TransferInfo
                            {
                                Type = package.Transportation.Transfers.Type ?? string.Empty,
                                Included = package.Transportation.Transfers.Included
                            }
                        };

                    var cancellationPolicy = package.CancellationPolicy is null
                        ? new CancellationPolicyInfo { FreeCancellation = false, Deadline = string.Empty, Penalty = string.Empty }
                        : new CancellationPolicyInfo
                        {
                            FreeCancellation = package.CancellationPolicy.FreeCancellation,
                            Deadline = package.CancellationPolicy.Deadline ?? string.Empty,
                            Penalty = package.CancellationPolicy.Penalty ?? string.Empty
                        };

                    var availability = package.Availability is null
                        ? new AvailabilityInfo { Status = string.Empty, RemainingSlots = 0 }
                        : new AvailabilityInfo
                        {
                            Status = package.Availability.Status ?? string.Empty,
                            RemainingSlots = package.Availability.RemainingSlots
                        };

                    var dto = new TourPackageDto(
                        id: Guid.NewGuid(),
                        externalId: package.PackageId,
                        name: package.Name,
                        price: price,
                        description: package.Description ?? string.Empty,
                        category: ProductCategory.TourPackage,
                        provider: "booking.com",
                        imageUrl: package.Images?.FirstOrDefault() ?? string.Empty,
                        createdAt: DateTime.UtcNow,
                        updatedAt: DateTime.UtcNow,
                        destination: destination,
                        duration: duration,
                        inclusions: package.Inclusions ?? new List<string>(),
                        exclusions: package.Exclusions ?? new List<string>(),
                        departureDates: package.DepartureDates?.Select(d => DateTime.Parse(d)).ToList() ?? new List<DateTime>(),
                        accommodation: accommodation,
                        transportation: transportation,
                        cancellationPolicy: cancellationPolicy,
                        availability: availability,
                        images: package.Images ?? new List<string>(),
                        termsAndConditions: package.TermsAndConditions ?? string.Empty,
                        lastUpdated: DateTime.TryParse(package.LastUpdated, out var lastUpd) ? lastUpd : DateTime.UtcNow
                    );

                    result.Add(dto);
                }
            }

            return result;
        }




        // Internal DTOs for deserialization
        private class ApiTourPackage
        {
            public required string PackageId { get; set; }
            public required string Name { get; set; }
            public string? Description { get; set; }
            public ApiDestination? Destination { get; set; }
            public ApiDuration? Duration { get; set; }
            public List<string>? Inclusions { get; set; }
            public List<string>? Exclusions { get; set; }
            public List<string>? DepartureDates { get; set; }
            public ApiPrice? Price { get; set; }
            public ApiAccommodation? Accommodation { get; set; }
            public ApiTransportation? Transportation { get; set; }
            public ApiCancellationPolicy? CancellationPolicy { get; set; }
            public ApiAvailability? Availability { get; set; }
            public List<string>? Images { get; set; }
            public string? TermsAndConditions { get; set; }
            public string? LastUpdated { get; set; }
        }

        private class ApiDestination
        {
            public string? Country { get; set; }
            public string? City { get; set; }
            public string? Resort { get; set; }
        }

        private class ApiDuration
        {
            public int Days { get; set; }
            public int Nights { get; set; }
        }

        private class ApiPrice
        {
            public decimal Amount { get; set; }
            public string? Currency { get; set; }
        }

        private class ApiAccommodation
        {
            public string? Type { get; set; }
            public int Rating { get; set; }
            public List<string>? Amenities { get; set; }
        }

        private class ApiTransportation
        {
            public ApiFlight? Flight { get; set; }
            public ApiTransfer? Transfers { get; set; }
        }

        private class ApiFlight
        {
            public string? Airline { get; set; }
            public string? Class { get; set; }
            public bool Included { get; set; }
        }

        private class ApiTransfer
        {
            public string? Type { get; set; }
            public bool Included { get; set; }
        }

        private class ApiCancellationPolicy
        {
            public bool FreeCancellation { get; set; }
            public string? Deadline { get; set; }
            public string? Penalty { get; set; }
        }

        private class ApiAvailability
        {
            public string? Status { get; set; }
            public int RemainingSlots { get; set; }
        }

    }
}