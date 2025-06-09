using Core.Application.DTOs;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;

namespace Core.Application.Mappings.Tests
{
    [TestClass()]
    public class CustomProductMapperTests
    {
        [TestMethod()]
        public void FromDomainTest()
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

            var domain = new CustomProduct(
                externalId: "CUST123",
                name: "Custom Leather Bag",
                price: Price.Create(199.99m, "USD"),
                description: "Handcrafted leather bag with custom engraving",
                category: ProductCategory.Custom,
                provider: "LeatherCraftCo",
                availability: new AvailabilityInfo
                {
                    Status = "Available",
                    RemainingSlots = 12
                },
                attributes: attributes
            )
            {
                Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                CreatedAt = DateTime.Parse("2024-01-15T09:30:00Z"),
                UpdatedAt = DateTime.Parse("2024-02-20T14:15:00Z"),
                ImageUrl = "https://example.com/images/leather-bag.jpg"
            };

            // Act
            CustomProductDto dto = CustomProductMapper.FromDomain(domain);

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), dto.Id);
            Assert.AreEqual("CUST123", dto.ExternalId);
            Assert.AreEqual("Custom Leather Bag", dto.Name);
            Assert.AreEqual(199.99m, dto.Price.Amount);
            Assert.AreEqual("USD", dto.Price.Currency.ToString());
            Assert.AreEqual("Handcrafted leather bag with custom engraving", dto.Description);
            Assert.AreEqual(ProductCategory.Custom, dto.Category);
            Assert.AreEqual("LeatherCraftCo", dto.Provider);
            Assert.AreEqual("https://example.com/images/leather-bag.jpg", dto.ImageUrl);
            Assert.AreEqual(DateTime.Parse("2024-01-15T09:30:00Z"), dto.CreatedAt);
            Assert.AreEqual(DateTime.Parse("2024-02-20T14:15:00Z"), dto.UpdatedAt);
            Assert.AreEqual("Available", dto.Availability.Status);
            Assert.AreEqual(12, dto.Availability.RemainingSlots);

            // Check attributes
            Assert.AreEqual(5, dto.Attributes.Count);
            Assert.AreEqual("Leather", dto.Attributes["Material"]);
            Assert.AreEqual("Black", dto.Attributes["Color"]);
            Assert.AreEqual(1.5, dto.Attributes["Weight"]);
            Assert.AreEqual(true, dto.Attributes["IsCustomizable"]);
            CollectionAssert.AreEqual(new List<int> { 10, 20, 30 }, (List<int>)dto.Attributes["Dimensions"]);
        }

        [TestMethod()]
        public void ToDomainTest()
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

            var dto = new CustomProductDto(
                id: Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                externalId: "CUST123",
                name: "Custom Leather Bag",
                price: Price.Create(199.99m, "USD"),
                availability: new AvailabilityInfo
                {
                    Status = "Available",
                    RemainingSlots = 12
                },
                description: "Handcrafted leather bag with custom engraving",
                category: ProductCategory.Custom,
                provider: "LeatherCraftCo",
                imageUrl: "https://example.com/images/leather-bag.jpg",
                createdAt: DateTime.Parse("2024-01-15T09:30:00Z"),
                updatedAt: DateTime.Parse("2024-02-20T14:15:00Z"),
                attributes: attributes
            );

            // Act
            CustomProduct domain = CustomProductMapper.ToDomain(dto);

            // Assert
            Assert.IsNotNull(domain);
            Assert.AreSame(domain.GetType(), typeof(Core.Domain.Entities.CustomProduct));

            Assert.AreEqual(Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), domain.Id);
            Assert.AreEqual("CUST123", domain.ExternalId);
            Assert.AreEqual("Custom Leather Bag", domain.Name);
            Assert.AreEqual(199.99m, domain.Price.Amount);
            Assert.AreEqual("USD", domain.Price.Currency.ToString());
            Assert.AreEqual("Handcrafted leather bag with custom engraving", domain.Description);
            Assert.AreEqual(ProductCategory.Custom, domain.Category);
            Assert.AreEqual("LeatherCraftCo", domain.Provider);
            Assert.AreEqual("https://example.com/images/leather-bag.jpg", domain.ImageUrl);
            Assert.AreEqual(DateTime.Parse("2024-01-15T09:30:00Z"), domain.CreatedAt);
            Assert.AreEqual(DateTime.Parse("2024-02-20T14:15:00Z"), domain.UpdatedAt);
            Assert.AreEqual("Available", domain.Availability.Status);
            Assert.AreEqual(12, domain.Availability.RemainingSlots);

            // Check attributes
            Assert.AreEqual(5, domain.Attributes.Count);
            Assert.AreEqual("Leather", domain.Attributes["Material"]);
            Assert.AreEqual("Black", domain.Attributes["Color"]);
            Assert.AreEqual(1.5, domain.Attributes["Weight"]);
            Assert.AreEqual(true, domain.Attributes["IsCustomizable"]);
            CollectionAssert.AreEqual(new List<int> { 10, 20, 30 }, (List<int>)domain.Attributes["Dimensions"]);
        }
    }
}