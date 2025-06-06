using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using System;
using System.Collections.Generic;

namespace Core.Domain.Entities.Tests
{
    [TestClass()]
    public class HolidayPackageTests
    {
        [TestMethod()]
        public void HolidayPackage_Creation_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var holidayPackage = new HolidayPackage(
                externalId: "HLD12345",
                provider: "expedia.com",
                name: "Romantic Paris Escape",
                price: Price.Create(1999.99m, "EUR"),
                availability: new AvailabilityInfo("Available", 5),
                description: "5-night romantic stay in Paris",
                property: new HolidayPackage.PropertyInfo
                {
                    Id = "PROP123",
                    Name = "Charming Hotel Paris",
                    Rating = 4.5,
                    Type = "Hotel",
                    Location = new HolidayPackage.LocationInfo
                    {
                        Country = "France",
                        City = "Paris"
                    }
                },
                roomOptions: new List<HolidayPackage.RoomOption>(),
                inclusions: new List<string> { "Breakfast", "City tour" },
                specialOffers: new List<SpecialOffer>(),
                cancellationPolicy: new CancellationPolicyInfo(),
                images: new List<string> { "paris1.jpg" },
                lastUpdated: DateTime.Parse("2024-04-01")
            );

            // Assert
            Assert.AreEqual("HLD12345", holidayPackage.ExternalId);
            Assert.AreEqual("expedia.com", holidayPackage.Provider);
            Assert.AreEqual("Romantic Paris Escape", holidayPackage.Name);
            Assert.AreEqual("5-night romantic stay in Paris", holidayPackage.Description);
            Assert.AreEqual("France", holidayPackage.Property.Location.Country);
            Assert.AreEqual("Paris", holidayPackage.Property.Location.City);
            CollectionAssert.AreEqual(new List<string> { "Breakfast", "City tour" }, holidayPackage.Inclusions);
            Assert.AreEqual("Available", holidayPackage.Availability.Status);
            Assert.AreEqual(5, holidayPackage.Availability.RemainingSlots);
            CollectionAssert.AreEqual(new List<string> { "paris1.jpg" }, holidayPackage.Images);
            Assert.AreEqual(DateTime.Parse("2024-04-01"), holidayPackage.LastUpdated);
            Assert.AreEqual("Hotel", holidayPackage.Property.Type);
            Assert.AreEqual(4.5, holidayPackage.Property.Rating);
        }

        
    }
}