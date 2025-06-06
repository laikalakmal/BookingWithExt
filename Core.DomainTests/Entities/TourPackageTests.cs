using Core.Domain.Entities.SupportClasses;

namespace Core.Domain.Entities.Tests
{
    [TestClass()]
    public class TourPackageTests
    {
        [TestMethod()]
        public void TourPackage_Creation_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var tourPackage = new TourPackage(
                externalId: "PKG12345",
                provider: "booking.com",
                name: "Luxury Maldives Getaway",
                price: Price.Create(2999.99m, "USD"),
                description: "7-day all-inclusive resort stay",
                destination: new DestinationInfo { Country = "Maldives" },
                duration: new DurationInfo { Days = 7, Nights = 6 },
                inclusions: new List<string> { "All meals", "Transfers" },
                exclusions: new List<string> { "Spa treatments" },
                departureDates: new List<DateTime> { DateTime.Parse("2024-06-15") },
                accommodation: new AccommodationInfo { Type = "Resort", Rating = 5 },
                transportation: new TransportationInfo(),
                cancellationPolicy: new CancellationPolicyInfo(),
                availability: new AvailabilityInfo("Available", 10),
                images: new List<string> { "image1.jpg" },
                termsAndConditions: "Terms apply",
                lastUpdated: DateTime.Parse("2024-03-15")
            );

            // Assert
            Assert.AreEqual("PKG12345", tourPackage.ExternalId);
            Assert.AreEqual("booking.com", tourPackage.Provider);
            Assert.AreEqual("Luxury Maldives Getaway", tourPackage.Name);
            Assert.AreEqual("7-day all-inclusive resort stay", tourPackage.Description);
            Assert.AreEqual("Maldives", tourPackage.Destination.Country);
            Assert.AreEqual(7, tourPackage.Duration.Days);
            Assert.AreEqual(6, tourPackage.Duration.Nights);
            CollectionAssert.AreEqual(new List<string> { "All meals", "Transfers" }, tourPackage.Inclusions);
            CollectionAssert.AreEqual(new List<string> { "Spa treatments" }, tourPackage.Exclusions);
            Assert.AreEqual(DateTime.Parse("2024-06-15"), tourPackage.DepartureDates[0]);
            Assert.AreEqual("Resort", tourPackage.Accommodation.Type);
            Assert.AreEqual(5, tourPackage.Accommodation.Rating);
            Assert.AreEqual("Available", tourPackage.Availability.Status);
            Assert.AreEqual(10, tourPackage.Availability.RemainingSlots);
            CollectionAssert.AreEqual(new List<string> { "image1.jpg" }, tourPackage.Images);
            Assert.AreEqual("Terms apply", tourPackage.TermsAndConditions);
            Assert.AreEqual(DateTime.Parse("2024-03-15"), tourPackage.LastUpdated);
        }

        
    }
}

