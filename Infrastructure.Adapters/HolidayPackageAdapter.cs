using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Infrastructure.Adapters
{
    public class HolidayPackageAdapter : IExternalProductApiAdapter
    {
        public string AdapterName => "agoda.com";

        public async Task<List<ProductDto>> FetchProductsAsync()
        {
            
            var apiUrl = "https://67c6dc0b-2e55-4479-9fa6-af5934f21e03.mock.pstmn.io/holidays";
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
                            ? new ApiProperty()
                            : new ApiProperty
                            {
                                Id = package.Property.Id,
                                Name = package.Property.Name,
                                Rating = package.Property.Rating,
                                Type = package.Property.Type,
                                Amenities = package.Property.Amenities ?? new List<string>(),
                                Location = package.Property.Location is null
                                    ? null
                                    : new ApiLocation
                                    {
                                        Country = package.Property.Location.Country,
                                        Island = package.Property.Location.Island,
                                        Coordinates = package.Property.Location.Coordinates is null
                                            ? null
                                            : new ApiCoordinates
                                            {
                                                Latitude = package.Property.Location.Coordinates.Latitude,
                                                Longitude = package.Property.Location.Coordinates.Longitude
                                            },
                                        TransferTime = package.Property.Location.TransferTime
                                    }
                            };

                        var roomOptions = new List<ApiRoomOption>();
                        if (package.RoomOptions != null)
                        {
                            foreach (var apiRoom in package.RoomOptions)
                            {
                                roomOptions.Add(new ApiRoomOption
                                {
                                    RoomType = apiRoom.RoomType,
                                    MaxOccupancy = apiRoom.MaxOccupancy,
                                    Size = apiRoom.Size,
                                    BedType = apiRoom.BedType,
                                    View = apiRoom.View,
                                    Amenities = apiRoom.Amenities ?? new List<string>(),
                                    Price = apiRoom.Price is null
                                        ? null
                                        : new ApiRoomPrice
                                        {
                                            PerNight = apiRoom.Price.PerNight,
                                            Total = apiRoom.Price.Total,
                                            Currency = apiRoom.Price.Currency,
                                            MealPlan = apiRoom.Price.MealPlan
                                        }
                                });
                            }
                        }

                        var specialOffers = new List<SpecialOffer>();
                        if (package.SpecialOffers != null)
                        {
                            foreach (var apiOffer in package.SpecialOffers)
                            {
                                specialOffers.Add(new SpecialOffer
                                {
                                    Name = apiOffer.Name ?? "",
                                    Description = apiOffer.Description ?? "",
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
                                Deadline = package.CancellationPolicy.Deadline ?? string.Empty,
                                Penalty = package.CancellationPolicy.Penalty ?? string.Empty
                            };

                        var firstRoom = package.RoomOptions?.FirstOrDefault();
                        var price = firstRoom?.Price != null
                            ? Price.Create(
                                firstRoom.Price.Total,
                                firstRoom.Price.Currency ?? "USD")
                            : Price.Create(0m, "USD");

                        var availability = new AvailabilityInfo("contact us", 100) { IsAvailable = true }; // since availability is not provided in the API response, we set a default value

                        Dictionary<string, object> attributes = [];
                        attributes.Add("property", property);
                        attributes.Add("roomOptions", roomOptions);
                        attributes.Add("specialOffers", specialOffers);
                        attributes.Add("cancellationPolicy", cancellationPolicy);
                        if (firstRoom != null) attributes.Add("firstRoom", firstRoom);


                        ProductDto dto = new(
                            id: Guid.NewGuid(),
                            externalId: package.PackageId,
                            name: package.Name,
                            price: price,
                            availability: availability,
                            description: package.Description ?? string.Empty,
                            category: ProductCategory.HolidayPackage,
                            provider: AdapterName,
                            imageUrl: package.Images ?? [],
                            createdAt: DateTime.UtcNow,
                            updatedAt: DateTime.UtcNow
                            )
                        {
                            Attributes = attributes,
                        }
                    ;

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

        public Task<ProductDto?> FetchProductByIdAsync(string externalId)
        {
            // this was done like this because the mock API does not support fetching a single product by ID directly.
            // so we fetch all products and filter them by externalId to mimic the behavior of fetching a single product.
            var products = FetchProductsAsync();

            if (products == null || products.Result.Count == 0)
            {
                throw new HttpRequestException("Failed to retrieve any products from the holiday package API.");

            }
            else
            {
                var product = products.Result.FirstOrDefault(p => p.ExternalId == externalId);
                return Task.FromResult(product);
            }


        }

        public async Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto productDto, int quantity)
        {
            // there should be a api call for purchasing a product from external API.
            //since i'm working with mock api i will temporarly return a mock purchase response.
            // in real use there should be a api call to purchase and if its data should be passed to the our API endpoint that respoinsible for purchasing product.

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            var product = FetchProductByIdAsync(productDto.ExternalId).Result;

            if (product == null)
            {
                return new PurchaseResponseDto(productDto.ExternalId)
                {
                    IsSuccess = false,
                    Message = "Product not found",
                    Provider = AdapterName,
                };
            }
            //check if product is available for purchase
            if (!product.Availability.IsAvailable)
            {
                return new PurchaseResponseDto(productDto.ExternalId)
                {
                    IsSuccess = false,
                    Message = "Product is not available for purchase",
                    Provider = AdapterName,
                };
            }

            // Mocking a purchase response, in real scenario this should be replaced with actual API call to purchase the product.



            var dto = new PurchaseResponseDto(Guid.NewGuid().ToString(), productDto.ExternalId)
            {
                ProductId = productDto.Id,
                ExternalId = productDto.ExternalId,
                Quantity = quantity,
                ConfirmationCode = null,
                CurrencyCode = "USD",
                IsSuccess = true,
                Message = "Your order is confirmed by holidayApi",
                PurchaseDate = DateTime.Now,
                TotalAmount = quantity * product.Price.Amount,

                Provider = AdapterName
            };

            return dto;

        }





        // Internal dto for deserialization
        private class ApiHolidayPackage
        {
            public required string PackageId { get; set; }
            public required string Name { get; set; }
            public string? Description { get; set; }
            public ApiProperty? Property { get; set; }

            public ApiAvailabilityInfo? Availability { get; set; }
            public List<ApiRoomOption>? RoomOptions { get; set; }
            public List<string>? Inclusions { get; set; }
            public List<ApiSpecialOffer>? SpecialOffers { get; set; }
            public ApiCancellationPolicy? CancellationPolicy { get; set; }
            public List<string>? Images { get; set; }
            public string? LastUpdated { get; set; }
        }


        private class ApiAvailabilityInfo
        {
            public string? Status { get; set; }
            public int RemainingSlots { get; set; }
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
