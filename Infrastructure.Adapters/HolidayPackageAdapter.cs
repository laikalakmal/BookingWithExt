using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Enums;
using Core.Domain.Entities.SupportClasses;

namespace Infrastructure.Adapters
{
    public class HolidayPackageAdapter : IExternalProductApiAdapter
    {
        public async Task<List<ProductDto>> FetchProductsAsync()
        {
            var apiUrl = "https://9fa670d1-1dd5-4cf6-819b-46fff26ce06f.mock.pstmn.io/hotels";
            var apiKey = "your-api-key";
            var result = new List<ProductDto>();

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

                var response = await httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var apiPackages = System.Text.Json.JsonSerializer.Deserialize<List<ApiHolidayPackage>>(json, options);

                if (apiPackages != null)
                {
                    foreach (var package in apiPackages)
                    {
                        var property = package.Property is null
                            ? new HolidayPackageDto.PropertyInfo()
                            : new HolidayPackageDto.PropertyInfo
                            {
                                Id = package.Property.Id,
                                Name = package.Property.Name,
                                Rating = package.Property.Rating,
                                Type = package.Property.Type,
                                Amenities = package.Property.Amenities ?? new List<string>(),
                                Location = package.Property.Location is null
                                    ? null
                                    : new HolidayPackageDto.LocationInfo
                                    {
                                        Country = package.Property.Location.Country,
                                        Island = package.Property.Location.Island,
                                        Coordinates = package.Property.Location.Coordinates is null
                                            ? null
                                            : new HolidayPackageDto.Coordinates
                                            {
                                                Latitude = package.Property.Location.Coordinates.Latitude,
                                                Longitude = package.Property.Location.Coordinates.Longitude
                                            },
                                        TransferTime = package.Property.Location.TransferTime
                                    }
                            };

                        var roomOptions = new List<HolidayPackageDto.RoomOption>();
                        if (package.RoomOptions != null)
                        {
                            foreach (var apiRoom in package.RoomOptions)
                            {
                                roomOptions.Add(new HolidayPackageDto.RoomOption
                                {
                                    RoomType = apiRoom.RoomType,
                                    MaxOccupancy = apiRoom.MaxOccupancy,
                                    Size = apiRoom.Size,
                                    BedType = apiRoom.BedType,
                                    View = apiRoom.View,
                                    Amenities = apiRoom.Amenities ?? new List<string>(),
                                    Price = apiRoom.Price is null
                                        ? null
                                        : new HolidayPackageDto.RoomPrice
                                        {
                                            PerNight = apiRoom.Price.PerNight,
                                            Total = apiRoom.Price.Total,
                                            Currency = apiRoom.Price.Currency,
                                            MealPlan = apiRoom.Price.MealPlan
                                        }
                                });
                            }
                        }

                        var specialOffers = new List<HolidayPackageDto.SpecialOffer>();
                        if (package.SpecialOffers != null)
                        {
                            foreach (var apiOffer in package.SpecialOffers)
                            {
                                specialOffers.Add(new HolidayPackageDto.SpecialOffer
                                {
                                    Name = apiOffer.Name,
                                    Description = apiOffer.Description,
                                    Discount = apiOffer.Discount.HasValue ? (decimal?)apiOffer.Discount.Value : null,
                                    ValidUntil = apiOffer.ValidUntil != null ? DateTime.TryParse(apiOffer.ValidUntil, out var date) ? date : null : null,
                                    RequiresVerification = apiOffer.RequiresVerification
                                });
                            }
                        }

                        var cancellationPolicy = package.CancellationPolicy is null
                            ? new CancellationPolicyInfo { FreeCancellation = false, Deadline = string.Empty, Penalty = string.Empty }
                            : new CancellationPolicyInfo
                            {
                                FreeCancellation = package.CancellationPolicy.FreeCancellation,
                                Deadline = package.CancellationPolicy.Deadline??string.Empty,
                                Penalty = package.CancellationPolicy.Penalty??string.Empty
                            };

                        var firstRoom = package.RoomOptions?.FirstOrDefault();
                        var price = firstRoom?.Price != null
                            ? Price.Create(
                                firstRoom.Price.Total,
                                firstRoom.Price.Currency ?? "USD")
                            : Price.Create(0m, "USD");

                        var dto = new HolidayPackageDto(
                            id: Guid.NewGuid(),
                            externalId: package.PackageId,
                            name: package.Name,
                            price: price,
                            description: package.Description ?? string.Empty,
                            category: ProductCategory.HolidayPackage,
                            provider: "agoda.com",
                            imageUrl: package.Images?.FirstOrDefault() ?? string.Empty,
                            createdAt: DateTime.UtcNow,
                            updatedAt: DateTime.UtcNow,
                            property: property,
                            roomOptions: roomOptions,
                            inclusions: package.Inclusions ?? new List<string>(),
                            specialOffers: specialOffers,
                            cancellationPolicy: cancellationPolicy,
                            images: package.Images ?? new List<string>(),
                            lastUpdated: DateTime.TryParse(package.LastUpdated, out var lastUpd) ? lastUpd : DateTime.UtcNow
                        );

                        result.Add(dto);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                throw ex.Message.Contains("404") ? new Exception("API endpoint not found.") : new Exception($"Failed to fetch products: {ex.Message}", ex);
            }
            catch (System.Text.Json.JsonException ex)
            {
                throw new Exception($"Failed to deserialize response: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }

            return result;
        }



        // Internal dto for deserialization
        private class ApiHolidayPackage
        {
            public required string PackageId { get; set; }
            public required string Name { get; set; }
            public string? Description { get; set; }
            public ApiProperty? Property { get; set; }
            public List<ApiRoomOption>? RoomOptions { get; set; }
            public List<string>? Inclusions { get; set; }
            public List<ApiSpecialOffer>? SpecialOffers { get; set; }
            public ApiCancellationPolicy? CancellationPolicy { get; set; }
            public List<string>? Images { get; set; }
            public string? LastUpdated { get; set; }
        }


        private class ApiProperty
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public double Rating { get; set; }
            public string? Type { get; set; }
            public ApiLocation? Location { get; set; }
            public List<string>? Amenities { get; set; }
        }

        private class ApiLocation
        {
            public string? Country { get; set; }
            public string? Island { get; set; }
            public ApiCoordinates? Coordinates { get; set; }
            public string? TransferTime { get; set; }
        }

        private class ApiCoordinates
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        private class ApiRoomOption
        {
            public string? RoomType { get; set; }
            public int MaxOccupancy { get; set; }
            public string? Size { get; set; }
            public string? BedType { get; set; }
            public string? View { get; set; }
            public List<string>? Amenities { get; set; }
            public ApiRoomPrice? Price { get; set; }
        }

        private class ApiRoomPrice
        {
            public decimal PerNight { get; set; }
            public decimal Total { get; set; }
            public string? Currency { get; set; }
            public string? MealPlan { get; set; }
        }

        private class ApiSpecialOffer
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public int? Discount { get; set; }
            public string? ValidUntil { get; set; }
            public bool RequiresVerification { get; set; }
        }

        private class ApiCancellationPolicy
        {
            public bool FreeCancellation { get; set; }
            public string? Deadline { get; set; }
            public string? Penalty { get; set; }
        }


    }
}
