using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;

namespace Core.Application.Mappings
{
    public class HolidayPackageMapper : IProductMapper<HolidayPackage, HolidayPackageDto>
    {
        public static HolidayPackageDto FromDomain(HolidayPackage product)
        {
            try
            {
                if (product is not HolidayPackage holidayPackage)
                    throw new ArgumentException("Product must be of type HolidayPackage", nameof(product));

                return new HolidayPackageDto(
                    id: holidayPackage.Id,
                    externalId: holidayPackage.ExternalId,
                    name: holidayPackage.Name,
                    price: holidayPackage.Price,
                    description: holidayPackage.Description ?? string.Empty,
                    category: holidayPackage.Category,
                    provider: holidayPackage.Provider,
                    imageUrl: holidayPackage.Images?.FirstOrDefault() ?? string.Empty,
                    createdAt: holidayPackage.CreatedAt,
                    updatedAt: holidayPackage.UpdatedAt,
                    property: holidayPackage.Property is null
                        ? new HolidayPackageDto.PropertyInfo()
                        : new HolidayPackageDto.PropertyInfo
                        {
                            Id = holidayPackage.Property.Id,
                            Name = holidayPackage.Property.Name,
                            Rating = holidayPackage.Property.Rating,
                            Type = holidayPackage.Property.Type,
                            Location = holidayPackage.Property.Location is null
                                ? new HolidayPackageDto.LocationInfo()
                                : new HolidayPackageDto.LocationInfo
                                {
                                    Country = holidayPackage.Property.Location.Country,
                                    Island = holidayPackage.Property.Location.Island,
                                    City = holidayPackage.Property.Location.City,
                                    Region = holidayPackage.Property.Location.Region,
                                    SkiArea = holidayPackage.Property.Location.SkiArea,
                                    Neighborhood = holidayPackage.Property.Location.Neighborhood,
                                    Reserve = holidayPackage.Property.Location.Reserve,
                                    NearestTown = holidayPackage.Property.Location.NearestTown,
                                    Lagoon = holidayPackage.Property.Location.Lagoon,
                                    Landmarks = holidayPackage.Property.Location.Landmarks?.ToList(),
                                    Coordinates = holidayPackage.Property.Location.Coordinates is null
                                        ? new HolidayPackageDto.Coordinates()
                                        : new HolidayPackageDto.Coordinates
                                        {
                                            Latitude = holidayPackage.Property.Location.Coordinates.Latitude,
                                            Longitude = holidayPackage.Property.Location.Coordinates.Longitude
                                        },
                                    TransferTime = holidayPackage.Property.Location.TransferTime
                                },
                            Amenities = holidayPackage.Property.Amenities?.ToList()
                        },
                    roomOptions: holidayPackage.RoomOptions?.Select(ro => new HolidayPackageDto.RoomOption
                    {
                        RoomType = ro.RoomType,
                        MaxOccupancy = ro.MaxOccupancy,
                        Size = ro.Size,
                        BedType = ro.BedType,
                        View = ro.View,
                        Amenities = ro.Amenities?.ToList(),
                        Price = ro.Price is null
                            ? new HolidayPackageDto.RoomPrice()
                            : new HolidayPackageDto.RoomPrice
                            {
                                PerNight = ro.Price.PerNight,
                                Total = ro.Price.Total,
                                Currency = ro.Price.Currency,
                                MealPlan = ro.Price.MealPlan
                            }
                    }).ToList() ?? new List<HolidayPackageDto.RoomOption>(),
                    inclusions: holidayPackage.Inclusions?.ToList() ?? new List<string>(),
                    specialOffers: holidayPackage.SpecialOffers?.Select(so => new HolidayPackageDto.SpecialOffer
                    {
                        Name = so.Name,
                        Description = so.Description,
                        Discount = so.Discount,
                        ValidUntil = so.ValidUntil,
                        RequiresVerification = so.RequiresVerification,
                        PriceAddon = so.PriceAddon,
                        Currency = so.Currency,
                        Code = so.Code,
                        MinimumNights = so.MinimumNights,
                        ValidFor = so.ValidFor?.ToList()
                    }).ToList() ?? new List<HolidayPackageDto.SpecialOffer>(),
                    cancellationPolicy: holidayPackage.CancellationPolicy is null
                        ? new CancellationPolicyInfo()
                        : new CancellationPolicyInfo
                        {
                            FreeCancellation = holidayPackage.CancellationPolicy.FreeCancellation,
                            Deadline = holidayPackage.CancellationPolicy.Deadline,
                            Penalty = holidayPackage.CancellationPolicy.Penalty
                        },
                    images: holidayPackage.Images?.ToList() ?? new List<string>(),
                    lastUpdated: holidayPackage.LastUpdated
                );
            }
            catch (ArgumentException)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException($"Failed to map HolidayPackage to DTO: {ex.Message}", ex);
            }
        }

        public static HolidayPackage ToDomain(HolidayPackageDto productDto)
        {
            try
            {
                if (productDto is not HolidayPackageDto holidayPackageDto)
                    throw new ArgumentException("ProductDto must be of type HolidayPackageDto", nameof(productDto));

                var property = new HolidayPackage.PropertyInfo
                {
                    Id = holidayPackageDto.Property?.Id ?? string.Empty,
                    Name = holidayPackageDto.Property?.Name ?? string.Empty,
                    Rating = holidayPackageDto.Property?.Rating ?? 0,
                    Type = holidayPackageDto.Property?.Type ?? string.Empty,
                    Location = holidayPackageDto.Property?.Location == null ? new HolidayPackage.LocationInfo() : new HolidayPackage.LocationInfo
                    {
                        Country = holidayPackageDto.Property?.Location.Country ?? string.Empty,
                        Island = holidayPackageDto.Property?.Location.Island ?? string.Empty,
                        City = holidayPackageDto.Property?.Location.City ?? string.Empty,
                        Region = holidayPackageDto.Property?.Location.Region ?? string.Empty,
                        SkiArea = holidayPackageDto.Property?.Location.SkiArea ?? string.Empty,
                        Neighborhood = holidayPackageDto.Property?.Location.Neighborhood ?? string.Empty,
                        Reserve = holidayPackageDto.Property?.Location.Reserve ?? string.Empty,
                        NearestTown = holidayPackageDto.Property?.Location.NearestTown ?? string.Empty,
                        Lagoon = holidayPackageDto.Property?.Location.Lagoon ?? string.Empty,
                        Landmarks = holidayPackageDto.Property?.Location?.Landmarks?.ToList() ?? [],
                        Coordinates = holidayPackageDto.Property?.Location.Coordinates == null ? new Coordinates() : new Coordinates
                        {
                            Latitude = holidayPackageDto.Property.Location.Coordinates.Latitude,
                            Longitude = holidayPackageDto.Property.Location.Coordinates.Longitude
                        },
                        TransferTime = holidayPackageDto.Property?.Location.TransferTime ?? string.Empty
                    },
                    Amenities = holidayPackageDto.Property?.Amenities?.ToList() ?? []
                };

                var roomOptions = holidayPackageDto.RoomOptions is not null ? holidayPackageDto.RoomOptions?.Select(ro => new HolidayPackage.RoomOption
                {
                    RoomType = ro.RoomType ?? string.Empty,
                    MaxOccupancy = ro.MaxOccupancy,
                    Size = ro.Size ?? string.Empty,
                    BedType = ro.BedType ?? string.Empty,
                    View = ro.View ?? string.Empty,
                    Amenities = ro.Amenities?.ToList() ?? [],
                    Price = ro.Price == null ? new HolidayPackage.RoomPrice() : new HolidayPackage.RoomPrice
                    {
                        PerNight = ro.Price.PerNight,
                        Total = ro.Price.Total,
                        Currency = ro.Price.Currency ?? string.Empty,
                        MealPlan = ro.Price.MealPlan ?? string.Empty
                    }
                }).ToList() : [];

                var specialOffers = holidayPackageDto.SpecialOffers?.Select(so => new SpecialOffer
                {
                    Name = so.Name ?? string.Empty,
                    Description = so.Description ?? string.Empty,
                    Discount = so.Discount,
                    ValidUntil = so.ValidUntil,
                    RequiresVerification = so.RequiresVerification,
                    PriceAddon = so.PriceAddon,
                    Currency = so.Currency ?? string.Empty,
                    Code = so.Code ?? string.Empty,
                    MinimumNights = so.MinimumNights,
                    ValidFor = so.ValidFor?.ToList() ?? []
                }).ToList();

                var cancellationPolicy = holidayPackageDto.CancellationPolicy == null ? new CancellationPolicyInfo() : new CancellationPolicyInfo
                {
                    FreeCancellation = holidayPackageDto.CancellationPolicy.FreeCancellation,
                    Deadline = holidayPackageDto.CancellationPolicy.Deadline ?? string.Empty,
                    Penalty = holidayPackageDto.CancellationPolicy.Penalty ?? string.Empty
                };

                return new HolidayPackage(
                    externalId: holidayPackageDto.ExternalId,
                    name: holidayPackageDto.Name,
                    price: holidayPackageDto.Price,
                    description: holidayPackageDto.Description,
                    property: property,
                    roomOptions: roomOptions ?? [],
                    inclusions: holidayPackageDto.Inclusions?.ToList() ?? [],
                    specialOffers: specialOffers ?? [],
                    cancellationPolicy: cancellationPolicy,
                    images: holidayPackageDto.Images?.ToList() ?? [],
                    lastUpdated: holidayPackageDto.LastUpdated,
                    provider: holidayPackageDto.Provider);
            }
            catch (ArgumentException)
            {

                throw;
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException($"Failed to map DTO to HolidayPackage: {ex.Message}", ex);
            }
        }
    }
}
