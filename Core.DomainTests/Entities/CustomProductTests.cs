using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Domain.Entities.Tests
{
    [TestClass()]
    public class CustomProductTests
    {
        [TestMethod()]
        public void CustomProduct_Creation_SetsPropertiesCorrectly()
        {
            // Arrange
            var attributes = new Dictionary<string, object>
    {
        { "Material", "Leather" },
        { "Color", "Black" },
        { "Weight", 1.5 },
        { "IsCustomizable", true },
        { "Dimensions", new List<int> { 10, 20, 30 } }
    };

            var testId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow.AddDays(-5);
            var updatedAt = DateTime.UtcNow;

            // Act
            var customProduct = new CustomProduct(
                externalId: "CUST123",
                name: "Custom Leather Bag",
                price: Price.Create(199.99m, "USD"),
                description: "Handcrafted leather bag with custom engraving",
                category: ProductCategory.Custom,
                provider: "LeatherCraftCo",
                availability: new AvailabilityInfo("Available", 12),
                attributes: attributes
            )
            {
                Id = testId,
                ImageUrl = "https://example.com/images/leather-bag.jpg",
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            // Assert
            // Base Product properties
            Assert.AreEqual(testId, customProduct.Id);
            Assert.AreEqual("CUST123", customProduct.ExternalId);
            Assert.AreEqual("Custom Leather Bag", customProduct.Name);
            Assert.AreEqual(199.99m, customProduct.Price.Amount);
            Assert.AreEqual("USD", customProduct.Price.Currency.ToString());
            Assert.AreEqual("Handcrafted leather bag with custom engraving", customProduct.Description);
            Assert.AreEqual(ProductCategory.Custom, customProduct.Category);
            Assert.AreEqual("LeatherCraftCo", customProduct.Provider);
            Assert.AreEqual(createdAt, customProduct.CreatedAt);
            Assert.AreEqual(updatedAt, customProduct.UpdatedAt);

            // Availability
            Assert.AreEqual("Available", customProduct.Availability.Status);
            Assert.AreEqual(12, customProduct.Availability.RemainingSlots);
            Assert.IsTrue(customProduct.Availability.IsAvailable);

            // CustomProduct specific properties
            Assert.AreEqual("https://example.com/images/leather-bag.jpg", customProduct.ImageUrl);

            // Attributes
            Assert.AreEqual(5, customProduct.Attributes.Count);
            Assert.AreEqual("Leather", customProduct.Attributes["Material"]);
            Assert.AreEqual("Black", customProduct.Attributes["Color"]);
            Assert.AreEqual(1.5, customProduct.Attributes["Weight"]);
            Assert.AreEqual(true, customProduct.Attributes["IsCustomizable"]);
            CollectionAssert.AreEqual(new List<int> { 10, 20, 30 }, (List<int>)customProduct.Attributes["Dimensions"]);
        }
    }
}