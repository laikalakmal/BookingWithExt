using Core.Application.DTOs;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;

namespace Core.Application.Mappings.Tests
{
    [TestClass()]
    public class HolidayPackageMapperTests
    {
        [TestMethod()]
        public void FromDomainTest()
        {
            // Arrange
            var domain = new HolidayPackage(
                externalId: "HOTELPKG001",
                provider: "agoda.com",
                name: "Luxury Beachfront Escape - Maldives",
                price: Price.Create(3250m, "USD"),
                description: "5-night stay at a 5-star resort with private beach access",
                property: new HolidayPackage.PropertyInfo
                {
                    Id = "RESORT123",
                    Name = "Sunset Paradise Resort & Spa",
                    Rating = 5,
                    Type = "Luxury Resort",
                    Location = new HolidayPackage.LocationInfo
                    {
                        Country = "Maldives",
                        Island = "North Malé Atoll",
                        Coordinates = new Domain.Entities.SupportClasses.Coordinates
                        {
                            Latitude = 4.1750,
                            Longitude = 73.5089
                        },
                        TransferTime = "20 minutes by speedboat"
                    },
                    Amenities = new List<string>
                    {
                        "Infinity pool",
                        "Private beach",
                        "Spa center",
                        "4 restaurants",
                        "Water sports center",
                        "Free WiFi",
                        "24-hour room service"
                    }
                },
                roomOptions: new List<HolidayPackage.RoomOption>
                {
                    new HolidayPackage.RoomOption
                    {
                        RoomType = "Beach Villa",
                        MaxOccupancy = 2,
                        Size = "85 sqm",
                        BedType = "King bed",
                        View = "Beachfront",
                        Amenities = new List<string>
                        {
                            "Private plunge pool",
                            "Outdoor shower",
                            "Mini-bar",
                            "Nespresso machine"
                        },
                        Price = new HolidayPackage.RoomPrice
                        {
                            PerNight = 650,
                            Total = 3250,
                            Currency = "USD",
                            MealPlan = "Half Board"
                        }
                    },
                    new HolidayPackage.RoomOption
                    {
                        RoomType = "Overwater Bungalow",
                        MaxOccupancy = 3,
                        Size = "100 sqm",
                        BedType = "King bed + sofa bed",
                        View = "Ocean",
                        Amenities = new List<string>
                        {
                            "Glass floor panel",
                            "Direct lagoon access",
                            "Sun deck",
                            "Bathroom with ocean view"
                        },
                        Price = new HolidayPackage.RoomPrice
                        {
                            PerNight = 950,
                            Total = 4750,
                            Currency = "USD",
                            MealPlan = "Full Board"
                        }
                    }
                },
                inclusions: new List<string>
                {
                    "Return speedboat transfers",
                    "Daily breakfast",
                    "Complimentary snorkeling equipment",
                    "Welcome drink and fruit basket",
                    "Free non-motorized water sports"
                },
                specialOffers: new List<SpecialOffer>
                {
                    new SpecialOffer
                    {
                        Name = "Early Bird Discount",
                        Description = "Book 90 days in advance",
                        Discount = 15,
                        ValidUntil = DateTime.Parse("2024-06-30"),
                        RequiresVerification = false
                    },
                    new SpecialOffer
                    {
                        Name = "Honeymoon Package",
                        Description = "Free romantic dinner and spa credit",
                        RequiresVerification = true
                    }
                },
                cancellationPolicy: new CancellationPolicyInfo
                {
                    FreeCancellation = true,
                    Deadline = "7 days before arrival",
                    Penalty = "1 night charge for late cancellation"
                },
                images: new List<string>
                {
                    "https://hotelwebsite.com/images/maldives-resort1.jpg",
                    "https://hotelwebsite.com/images/maldives-room1.jpg",
                    "https://hotelwebsite.com/images/maldives-pool1.jpg"
                },
                lastUpdated: DateTime.Parse("2024-03-18T09:15:00Z")
            );

            // Act
            HolidayPackageDto dto = (HolidayPackageDto)HolidayPackageMapper.FromDomain(domain);

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual("HOTELPKG001", dto.ExternalId);
            Assert.AreEqual("Luxury Beachfront Escape - Maldives", dto.Name);
            Assert.AreEqual("5-night stay at a 5-star resort with private beach access", dto.Description);

            // Property assertions
            Assert.AreEqual("RESORT123", dto.Property.Id);
            Assert.AreEqual("Sunset Paradise Resort & Spa", dto.Property.Name);
            Assert.AreEqual(5, dto.Property.Rating);
            Assert.AreEqual("Luxury Resort", dto.Property.Type);

            // Location assertions
            Assert.AreEqual("Maldives", dto.Property.Location?.Country ?? string.Empty);
            Assert.AreEqual("North Malé Atoll", dto.Property.Location?.Island ?? string.Empty);
            Assert.AreEqual(4.1750, dto.Property.Location?.Coordinates?.Latitude);
            Assert.AreEqual(73.5089, dto.Property.Location?.Coordinates?.Longitude);
            Assert.AreEqual("20 minutes by speedboat", dto.Property.Location?.TransferTime ?? string.Empty);

            // Amenities assertions
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Infinity pool",
                    "Private beach",
                    "Spa center",
                    "4 restaurants",
                    "Water sports center",
                    "Free WiFi",
                    "24-hour room service"
                },
                dto.Property.Amenities
            );

            // Room options assertions
            Assert.AreEqual(2, dto.RoomOptions.Count);

            // First room option assertions
            Assert.AreEqual("Beach Villa", dto.RoomOptions[0].RoomType);
            Assert.AreEqual(2, dto.RoomOptions[0].MaxOccupancy);
            Assert.AreEqual("85 sqm", dto.RoomOptions[0].Size);
            Assert.AreEqual("King bed", dto.RoomOptions[0].BedType);
            Assert.AreEqual("Beachfront", dto.RoomOptions[0].View);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Private plunge pool",
                    "Outdoor shower",
                    "Mini-bar",
                    "Nespresso machine"
                },
                dto.RoomOptions[0].Amenities
            );
            Assert.AreEqual(650, dto.RoomOptions[0]?.Price?.PerNight);
            Assert.AreEqual(3250, dto.RoomOptions[0]?.Price?.Total);
            Assert.AreEqual("USD", dto.RoomOptions[0]?.Price?.Currency);
            Assert.AreEqual("Half Board", dto.RoomOptions[0]?.Price?.MealPlan);

            // Second room option assertions
            Assert.AreEqual("Overwater Bungalow", dto.RoomOptions[1].RoomType);
            Assert.AreEqual(3, dto.RoomOptions[1].MaxOccupancy);
            Assert.AreEqual("100 sqm", dto.RoomOptions[1].Size);
            Assert.AreEqual("King bed + sofa bed", dto.RoomOptions[1].BedType);
            Assert.AreEqual("Ocean", dto.RoomOptions[1].View);

            // Inclusions assertions
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Return speedboat transfers",
                    "Daily breakfast",
                    "Complimentary snorkeling equipment",
                    "Welcome drink and fruit basket",
                    "Free non-motorized water sports"
                },
                dto.Inclusions
            );

            // Special offers assertions
            Assert.AreEqual(2, dto.SpecialOffers.Count);
            Assert.AreEqual("Early Bird Discount", dto.SpecialOffers[0].Name);
            Assert.AreEqual("Book 90 days in advance", dto.SpecialOffers[0].Description);
            Assert.AreEqual(15, dto.SpecialOffers[0].Discount);
            Assert.AreEqual(DateTime.Parse("2024-06-30"), dto.SpecialOffers[0].ValidUntil);
            Assert.IsFalse(dto.SpecialOffers[0].RequiresVerification);

            Assert.AreEqual("Honeymoon Package", dto.SpecialOffers[1].Name);
            Assert.AreEqual("Free romantic dinner and spa credit", dto.SpecialOffers[1].Description);
            Assert.IsTrue(dto.SpecialOffers[1].RequiresVerification);

            // Cancellation policy assertions
            Assert.IsTrue(dto.CancellationPolicy.FreeCancellation);
            Assert.AreEqual("7 days before arrival", dto.CancellationPolicy.Deadline);
            Assert.AreEqual("1 night charge for late cancellation", dto.CancellationPolicy.Penalty);

            // Images assertions
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "https://hotelwebsite.com/images/maldives-resort1.jpg",
                    "https://hotelwebsite.com/images/maldives-room1.jpg",
                    "https://hotelwebsite.com/images/maldives-pool1.jpg"
                },
                dto.Images
            );

            // Last updated assertion
            Assert.AreEqual(DateTime.Parse("2024-03-18T09:15:00Z"), dto.LastUpdated);
        }

        [TestMethod()]
        public void ToDomainTest()
        {
            // Arrange
            var dto = new HolidayPackageDto(
                id: Guid.NewGuid(),
                externalId: "HOTELPKG001",
                name: "Luxury Beachfront Escape - Maldives",
                price: Price.Create(3250m, "USD"),
                description: "5-night stay at a 5-star resort with private beach access",
                category: Domain.Enums.ProductCategory.HolidayPackage,
                provider: "booking.com",
                imageUrl: "https://hotelwebsite.com/images/maldives-resort1.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                property: new HolidayPackageDto.PropertyInfo
                {
                    Id = "RESORT123",
                    Name = "Sunset Paradise Resort & Spa",
                    Rating = 5,
                    Type = "Luxury Resort",
                    Location = new HolidayPackageDto.LocationInfo
                    {
                        Country = "Maldives",
                        Island = "North Malé Atoll",
                        Coordinates = new HolidayPackageDto.Coordinates
                        {
                            Latitude = 4.1750,
                            Longitude = 73.5089
                        },
                        TransferTime = "20 minutes by speedboat"
                    },
                    Amenities = new List<string>
                    {
                        "Infinity pool",
                        "Private beach",
                        "Spa center",
                        "4 restaurants",
                        "Water sports center",
                        "Free WiFi",
                        "24-hour room service"
                    }
                },
                roomOptions: new List<HolidayPackageDto.RoomOption>
                {
                    new HolidayPackageDto.RoomOption
                    {
                        RoomType = "Beach Villa",
                        MaxOccupancy = 2,
                        Size = "85 sqm",
                        BedType = "King bed",
                        View = "Beachfront",
                        Amenities = new List<string>
                        {
                            "Private plunge pool",
                            "Outdoor shower",
                            "Mini-bar",
                            "Nespresso machine"
                        },
                        Price = new HolidayPackageDto.RoomPrice
                        {
                            PerNight = 650,
                            Total = 3250,
                            Currency = "USD",
                            MealPlan = "Half Board"
                        }
                    },
                    new HolidayPackageDto.RoomOption
                    {
                        RoomType = "Overwater Bungalow",
                        MaxOccupancy = 3,
                        Size = "100 sqm",
                        BedType = "King bed + sofa bed",
                        View = "Ocean",
                        Amenities = new List<string>
                        {
                            "Glass floor panel",
                            "Direct lagoon access",
                            "Sun deck",
                            "Bathroom with ocean view"
                        },
                        Price = new HolidayPackageDto.RoomPrice
                        {
                            PerNight = 950,
                            Total = 4750,
                            Currency = "USD",
                            MealPlan = "Full Board"
                        }
                    }
                },
                inclusions: new List<string>
                {
                    "Return speedboat transfers",
                    "Daily breakfast",
                    "Complimentary snorkeling equipment",
                    "Welcome drink and fruit basket",
                    "Free non-motorized water sports"
                },
                specialOffers: new List<HolidayPackageDto.SpecialOffer>
                {
                    new HolidayPackageDto.SpecialOffer
                    {
                        Name = "Early Bird Discount",
                        Description = "Book 90 days in advance",
                        Discount = 15,
                        ValidUntil = DateTime.Parse("2024-06-30"),
                        RequiresVerification = false
                    },
                    new HolidayPackageDto.SpecialOffer
                    {
                        Name = "Honeymoon Package",
                        Description = "Free romantic dinner and spa credit",
                        RequiresVerification = true
                    }
                },
                cancellationPolicy: new CancellationPolicyInfo
                {
                    FreeCancellation = true,
                    Deadline = "7 days before arrival",
                    Penalty = "1 night charge for late cancellation"
                },
                images: new List<string>
                {
                    "https://hotelwebsite.com/images/maldives-resort1.jpg",
                    "https://hotelwebsite.com/images/maldives-room1.jpg",
                    "https://hotelwebsite.com/images/maldives-pool1.jpg"
                },
                lastUpdated: DateTime.Parse("2024-03-18T09:15:00Z")
            );

            // Act
            HolidayPackage domain = (HolidayPackage)HolidayPackageMapper.ToDomain(dto);

            // Assert
            Assert.IsNotNull(domain);
            Assert.AreSame(domain.GetType(), typeof(Core.Domain.Entities.HolidayPackage));

            Assert.AreEqual("HOTELPKG001", domain.ExternalId);
            Assert.AreEqual("Luxury Beachfront Escape - Maldives", domain.Name);
            Assert.AreEqual("5-night stay at a 5-star resort with private beach access", domain.Description);

            // Property assertions
            Assert.AreEqual("RESORT123", domain.Property.Id);
            Assert.AreEqual("Sunset Paradise Resort & Spa", domain.Property.Name);
            Assert.AreEqual(5, domain.Property.Rating);
            Assert.AreEqual("Luxury Resort", domain.Property.Type);

            // Location assertions
            Assert.AreEqual("Maldives", domain.Property.Location.Country);
            Assert.AreEqual("North Malé Atoll", domain.Property.Location.Island);
            Assert.AreEqual(4.1750, domain.Property.Location.Coordinates.Latitude);
            Assert.AreEqual(73.5089, domain.Property.Location.Coordinates.Longitude);
            Assert.AreEqual("20 minutes by speedboat", domain.Property.Location.TransferTime);

            // Amenities assertions
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Infinity pool",
                    "Private beach",
                    "Spa center",
                    "4 restaurants",
                    "Water sports center",
                    "Free WiFi",
                    "24-hour room service"
                },
                domain.Property.Amenities
            );

            // Room options assertions
            Assert.AreEqual(2, domain.RoomOptions.Count);

            // First room option assertions
            Assert.AreEqual("Beach Villa", domain.RoomOptions[0].RoomType);
            Assert.AreEqual(2, domain.RoomOptions[0].MaxOccupancy);
            Assert.AreEqual("85 sqm", domain.RoomOptions[0].Size);
            Assert.AreEqual("King bed", domain.RoomOptions[0].BedType);
            Assert.AreEqual("Beachfront", domain.RoomOptions[0].View);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Private plunge pool",
                    "Outdoor shower",
                    "Mini-bar",
                    "Nespresso machine"
                },
                domain.RoomOptions[0].Amenities
            );
            Assert.AreEqual(650, domain.RoomOptions[0].Price.PerNight);
            Assert.AreEqual(3250, domain.RoomOptions[0].Price.Total);
            Assert.AreEqual("USD", domain.RoomOptions[0].Price.Currency);
            Assert.AreEqual("Half Board", domain.RoomOptions[0].Price.MealPlan);

            // Second room option assertions
            Assert.AreEqual("Overwater Bungalow", domain.RoomOptions[1].RoomType);
            Assert.AreEqual(3, domain.RoomOptions[1].MaxOccupancy);
            Assert.AreEqual("100 sqm", domain.RoomOptions[1].Size);
            Assert.AreEqual("King bed + sofa bed", domain.RoomOptions[1].BedType);
            Assert.AreEqual("Ocean", domain.RoomOptions[1].View);

            // Inclusions assertions
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Return speedboat transfers",
                    "Daily breakfast",
                    "Complimentary snorkeling equipment",
                    "Welcome drink and fruit basket",
                    "Free non-motorized water sports"
                },
                domain.Inclusions
            );

            // Special offers assertions
            Assert.AreEqual(2, domain.SpecialOffers.Count);
            Assert.AreEqual("Early Bird Discount", domain.SpecialOffers[0].Name);
            Assert.AreEqual("Book 90 days in advance", domain.SpecialOffers[0].Description);
            Assert.AreEqual(15, domain.SpecialOffers[0].Discount);
            Assert.AreEqual(DateTime.Parse("2024-06-30"), domain.SpecialOffers[0].ValidUntil);
            Assert.IsFalse(domain.SpecialOffers[0].RequiresVerification);

            Assert.AreEqual("Honeymoon Package", domain.SpecialOffers[1].Name);
            Assert.AreEqual("Free romantic dinner and spa credit", domain.SpecialOffers[1].Description);
            Assert.IsTrue(domain.SpecialOffers[1].RequiresVerification);

            // Cancellation policy assertions
            Assert.IsTrue(domain.CancellationPolicy.FreeCancellation);
            Assert.AreEqual("7 days before arrival", domain.CancellationPolicy.Deadline);
            Assert.AreEqual("1 night charge for late cancellation", domain.CancellationPolicy.Penalty);

            // Images assertions
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "https://hotelwebsite.com/images/maldives-resort1.jpg",
                    "https://hotelwebsite.com/images/maldives-room1.jpg",
                    "https://hotelwebsite.com/images/maldives-pool1.jpg"
                },
                domain.Images
            );

            // Last updated assertion
            Assert.AreEqual(DateTime.Parse("2024-03-18T09:15:00Z"), domain.LastUpdated);
        }
    }
}
