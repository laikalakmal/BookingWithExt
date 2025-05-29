using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Mappings.Tests
{
    [TestClass()]
    public class TourPackageMapperTests
    {
        [TestMethod()]
        public void FromDomainTest()
        {
            // Arrange
            var domain = new TourPackage(

                externalId: "PKG12345",
                provider: "booking.com",
                name: "Luxury Maldives Getaway",
                price: Price.Create(2999.99m, "USD"),
                description: "7-day all-inclusive resort stay with private beach access",
                destination: new DestinationInfo
                {
                    Country = "Maldives",
                    City = "North Malé Atoll",
                    Resort = "Paradise Island Resort"
                },
                duration: new DurationInfo
                {
                    Days = 7,
                    Nights = 6
                },
                inclusions: new List<string>
                {
                    "Return flights (economy class)",
                    "Ocean-view villa accommodation",
                    "All meals and beverages",
                    "Airport transfers",
                    "Daily housekeeping",
                    "Free WiFi",
                    "Snorkeling equipment rental"
                },
                exclusions: new List<string>
                {
                    "Spa treatments",
                    "Scuba diving excursions",
                    "Travel insurance"
                },
                departureDates: new List<DateTime>
                {
                    new DateTime(2024, 6, 15),
                    new DateTime(2024, 7, 10),
                    new DateTime(2024, 8, 5)
                },
                accommodation: new AccommodationInfo
                {
                    Type = "Beach Villa",
                    Rating = 5,
                    Amenities = new List<string>
                    {
                        "King-size bed",
                        "Private plunge pool",
                        "Air conditioning",
                        "Mini-bar",
                        "En-suite bathroom"
                    }
                },
                transportation: new TransportationInfo
                {
                    Flight = new FlightInfo
                    {
                        Airline = "Emirates",
                        Class = "Economy",
                        Included = true
                    },
                    Transfers = new TransferInfo
                    {
                        Type = "Speedboat",
                        Included = true
                    }
                },
                cancellationPolicy: new Domain.Entities.SupportClasses.CancellationPolicyInfo
                {
                    FreeCancellation = true,
                    Deadline = "14 days before departure",
                    Penalty = "Full payment if cancelled within 7 days"
                },
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo
                {
                    Status = "Available",
                    RemainingSlots = 8
                },
                images: new List<string>
                {
                    "https://example.com/images/maldives1.jpg",
                    "https://example.com/images/maldives2.jpg",
                    "https://example.com/images/maldives3.jpg"
                },
                termsAndConditions: "Prices are subject to change based on availability. Valid passport required. Some restrictions may apply.",
                lastUpdated: DateTime.Parse("2024-03-15T10:30:00Z")
            );

            // Act
            TourPackageDto dto = (TourPackageDto)TourPackageMapper.FromDomain(domain);

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual("PKG12345", dto.ExternalId);
            Assert.AreEqual("Luxury Maldives Getaway", dto.Name);
            Assert.AreEqual("Maldives", dto.Destination.Country);
            Assert.AreEqual("North Malé Atoll", dto.Destination.City);
            Assert.AreEqual("Paradise Island Resort", dto.Destination.Resort);
            Assert.AreEqual(7, dto.Duration.Days);
            Assert.AreEqual(6, dto.Duration.Nights);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Return flights (economy class)",
                    "Ocean-view villa accommodation",
                    "All meals and beverages",
                    "Airport transfers",
                    "Daily housekeeping",
                    "Free WiFi",
                    "Snorkeling equipment rental"
                },
                dto.Inclusions
            );
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Spa treatments",
                    "Scuba diving excursions",
                    "Travel insurance"
                },
                dto.Exclusions
            );
            CollectionAssert.AreEqual(
                new List<DateTime>
                {
                    new DateTime(2024, 6, 15),
                    new DateTime(2024, 7, 10),
                    new DateTime(2024, 8, 5)
                },
                dto.DepartureDates
            );
            Assert.AreEqual("Beach Villa", dto.Accommodation.Type);
            Assert.AreEqual(5, dto.Accommodation.Rating);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "King-size bed",
                    "Private plunge pool",
                    "Air conditioning",
                    "Mini-bar",
                    "En-suite bathroom"
                },
                dto.Accommodation.Amenities
            );
            Assert.AreEqual("Emirates", dto.Transportation?.Flight?.Airline);
            Assert.AreEqual("Economy", dto.Transportation?.Flight?.Class);
            Assert.IsTrue(dto.Transportation?.Flight?.Included);
            Assert.AreEqual("Speedboat", dto.Transportation?.Transfers?.Type);
            Assert.IsTrue(dto.Transportation?.Transfers?.Included);
            Assert.IsTrue(dto.CancellationPolicy.FreeCancellation);
            Assert.AreEqual("14 days before departure", dto.CancellationPolicy.Deadline);
            Assert.AreEqual("Full payment if cancelled within 7 days", dto.CancellationPolicy.Penalty);
            Assert.AreEqual("Available", dto.Availability.Status);
            Assert.AreEqual(8, dto.Availability.RemainingSlots);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "https://example.com/images/maldives1.jpg",
                    "https://example.com/images/maldives2.jpg",
                    "https://example.com/images/maldives3.jpg"
                },
                dto.Images
            );
            Assert.AreEqual("Prices are subject to change based on availability. Valid passport required. Some restrictions may apply.", dto.TermsAndConditions);
            Assert.AreEqual(DateTime.Parse("2024-03-15T10:30:00Z"), dto.LastUpdated);
        }

        [TestMethod()]
        public void ToDomainTest()
        {
            // Arrange
            var dto = new Core.Application.DTOs.TourPackageDto(
                id: Guid.NewGuid(),
                externalId: "PKG12345",
                name: "Luxury Maldives Getaway",
                price: Price.Create(2999.99m, "USD"),
                description: "7-day all-inclusive resort stay with private beach access",
                category: Domain.Enums.ProductCategory.TourPackage,
                provider: "booking.com",
                imageUrl: "https://example.com/images/maldives1.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                destination: new Core.Application.DTOs.TourPackageDto.DestinationInfo
                {
                    Country = "Maldives",
                    City = "North Malé Atoll",
                    Resort = "Paradise Island Resort"
                },
                duration: new Core.Application.DTOs.TourPackageDto.DurationInfo
                {
                    Days = 7,
                    Nights = 6
                },
                inclusions: new List<string>
                {
                    "Return flights (economy class)",
                    "Ocean-view villa accommodation",
                    "All meals and beverages",
                    "Airport transfers",
                    "Daily housekeeping",
                    "Free WiFi",
                    "Snorkeling equipment rental"
                },
                exclusions: new List<string>
                {
                    "Spa treatments",
                    "Scuba diving excursions",
                    "Travel insurance"
                },
                departureDates: new List<DateTime>
                {
                    new DateTime(2024, 6, 15),
                    new DateTime(2024, 7, 10),
                    new DateTime(2024, 8, 5)
                },
                accommodation: new Core.Application.DTOs.TourPackageDto.AccommodationInfo
                {
                    Type = "Beach Villa",
                    Rating = 5,
                    Amenities = new List<string>
                    {
                        "King-size bed",
                        "Private plunge pool",
                        "Air conditioning",
                        "Mini-bar",
                        "En-suite bathroom"
                    }
                },
                transportation: new Core.Application.DTOs.TourPackageDto.TransportationInfo
                {
                    Flight = new Core.Application.DTOs.TourPackageDto.FlightInfo
                    {
                        Airline = "Emirates",
                        Class = "Economy",
                        Included = true
                    },
                    Transfers = new Core.Application.DTOs.TourPackageDto.TransferInfo
                    {
                        Type = "Speedboat",
                        Included = true
                    }
                },
                cancellationPolicy: new Domain.Entities.SupportClasses.CancellationPolicyInfo
                {
                    FreeCancellation = true,
                    Deadline = "14 days before departure",
                    Penalty = "Full payment if cancelled within 7 days"
                },
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo
                {
                    Status = "Available",
                    RemainingSlots = 8
                },
                images: new List<string>
                {
                    "https://example.com/images/maldives1.jpg",
                    "https://example.com/images/maldives2.jpg",
                    "https://example.com/images/maldives3.jpg"
                },
                termsAndConditions: "Prices are subject to change based on availability. Valid passport required. Some restrictions may apply.",
                lastUpdated: DateTime.Parse("2024-03-15T10:30:00Z")
            );

            // Act
            TourPackage domain = (TourPackage)TourPackageMapper.ToDomain(dto);

            // Assert
            Assert.IsNotNull(domain);
            Assert.AreSame(domain.GetType(), typeof(Core.Domain.Entities.TourPackage));

            Assert.AreEqual("PKG12345", domain.ExternalId);
            Assert.AreEqual("Luxury Maldives Getaway", domain.Name);
            Assert.AreEqual("Maldives", domain.Destination.Country);
            Assert.AreEqual("North Malé Atoll", domain.Destination.City);
            Assert.AreEqual("Paradise Island Resort", domain.Destination.Resort);
            Assert.AreEqual(7, domain.Duration.Days);
            Assert.AreEqual(6, domain.Duration.Nights);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Return flights (economy class)",
                    "Ocean-view villa accommodation",
                    "All meals and beverages",
                    "Airport transfers",
                    "Daily housekeeping",
                    "Free WiFi",
                    "Snorkeling equipment rental"
                },
                domain.Inclusions
            );
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "Spa treatments",
                    "Scuba diving excursions",
                    "Travel insurance"
                },
                domain.Exclusions
            );
            CollectionAssert.AreEqual(
                new List<DateTime>
                {
                    new DateTime(2024, 6, 15),
                    new DateTime(2024, 7, 10),
                    new DateTime(2024, 8, 5)
                },
                domain.DepartureDates
            );
            Assert.AreEqual("Beach Villa", domain.Accommodation.Type);
            Assert.AreEqual(5, domain.Accommodation.Rating);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "King-size bed",
                    "Private plunge pool",
                    "Air conditioning",
                    "Mini-bar",
                    "En-suite bathroom"
                },
                domain.Accommodation.Amenities
            );
            Assert.AreEqual("Emirates", domain.Transportation?.Flight?.Airline);
            Assert.AreEqual("Economy", domain.Transportation?.Flight?.Class);
            Assert.IsTrue(domain.Transportation?.Flight?.Included);
            Assert.AreEqual("Speedboat", domain.Transportation?.Transfers?.Type);
            Assert.IsTrue(domain.Transportation?.Transfers?.Included);
            Assert.IsTrue(domain.CancellationPolicy.FreeCancellation);
            Assert.AreEqual("14 days before departure", domain.CancellationPolicy.Deadline);
            Assert.AreEqual("Full payment if cancelled within 7 days", domain.CancellationPolicy.Penalty);
            Assert.AreEqual("Available", domain.Availability.Status);
            Assert.AreEqual(8, domain.Availability.RemainingSlots);
            CollectionAssert.AreEqual(
                new List<string>
                {
                    "https://example.com/images/maldives1.jpg",
                    "https://example.com/images/maldives2.jpg",
                    "https://example.com/images/maldives3.jpg"
                },
                domain.Images
            );
            Assert.AreEqual("Prices are subject to change based on availability. Valid passport required. Some restrictions may apply.", domain.TermsAndConditions);
            Assert.AreEqual(DateTime.Parse("2024-03-15T10:30:00Z"), domain.LastUpdated);
        }
    }
}