using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Application.Mappings;
using System;
using System.Collections.Generic;
using Core.Domain.Entities;
using Core.Application.DTOs;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using Core.Domain.ValueObjects;

namespace Core.Application.Mappings.Tests
{
    [TestClass()]
    public class ProductMapperTests
    {
        [TestMethod()]
        public void FromDomainTest()
        {
            // Arrange
            var product = new Product
            {
                Id = Guid.NewGuid(),
                ExternalId = "EXT123",
                Name = "Test Product",
                Price = Price.Create(99.99m, "USD"),
                Description = "Test Description",
                Category = ProductCategory.TourPackage,
                Provider = "TestProvider",
                Availability = new AvailabilityInfo { IsAvailable = true },
                ImageUrl = new List<string> { "http://example.com/image1.jpg", "http://example.com/image2.jpg" },
                CreatedAt = new DateTime(2023, 1, 1),
                UpdatedAt = new DateTime(2023, 1, 2),
                Attributes = new Dictionary<string, object> { { "color", "blue" }, { "size", "medium" } }
            };

            // Act
            var dto = ProductMapper.FromDomain(product);

            // Assert
            Assert.AreEqual(product.Id, dto.Id);
            Assert.AreEqual(product.ExternalId, dto.ExternalId);
            Assert.AreEqual(product.Name, dto.Name);
            Assert.AreEqual(product.Price.Amount, dto.Price.Amount);
            Assert.AreEqual(product.Price.Currency, dto.Price.Currency);
            Assert.AreEqual(product.Description, dto.Description);
            Assert.AreEqual(product.Category, dto.Category);
            Assert.AreEqual(product.Provider, dto.Provider);
            Assert.AreEqual(product.Availability.IsAvailable, dto.Availability.IsAvailable);
            Assert.AreEqual(product.CreatedAt, dto.CreatedAt);
            Assert.AreEqual(product.UpdatedAt, dto.UpdatedAt);
            CollectionAssert.AreEqual(product.ImageUrl, dto.ImageUrl);
            Assert.AreEqual(product.Attributes.Count, dto.Attributes.Count);
            Assert.AreEqual(product.Attributes["color"], dto.Attributes["color"]);
            Assert.AreEqual(product.Attributes["size"], dto.Attributes["size"]);
        }

        [TestMethod()]
        public void ToDomainTest()
        {
            // Arrange
            var productDto = new ProductDto(
                id: Guid.NewGuid(),
                externalId: "EXT456",
                name: "Test DTO Product",
                price: Price.Create( 149.99m, "EUR" ),
                availability: new AvailabilityInfo { IsAvailable = true },
                description: "Test DTO Description",
                category: ProductCategory.HolidayPackage,
                provider: "DTOProvider",
                imageUrl: new List<string> { "http://example.com/dto1.jpg", "http://example.com/dto2.jpg" },
                createdAt: new DateTime(2023, 2, 1),
                updatedAt: new DateTime(2023, 2, 2)
            );
            productDto.Attributes.Add("material", "leather");
            productDto.Attributes.Add("weight", 2.5);

            // Act
            var product = ProductMapper.ToDomain(productDto);

            // Assert
            Assert.AreEqual(productDto.Id, product.Id);
            Assert.AreEqual(productDto.ExternalId, product.ExternalId);
            Assert.AreEqual(productDto.Name, product.Name);
            Assert.AreEqual(productDto.Price.Amount, product.Price.Amount);
            Assert.AreEqual(productDto.Price.Currency, product.Price.Currency);
            Assert.AreEqual(productDto.Description, product.Description);
            Assert.AreEqual(productDto.Category, product.Category);
            Assert.AreEqual(productDto.Provider, product.Provider);
            Assert.AreEqual(productDto.Availability.IsAvailable, product.Availability.IsAvailable);
            Assert.AreEqual(productDto.CreatedAt, product.CreatedAt);
            Assert.AreEqual(productDto.UpdatedAt, product.UpdatedAt);
            CollectionAssert.AreEqual(productDto.ImageUrl, product.ImageUrl);
            Assert.AreEqual(productDto.Attributes.Count, product.Attributes.Count);
            Assert.AreEqual(productDto.Attributes["material"], product.Attributes["material"]);
            Assert.AreEqual(productDto.Attributes["weight"], product.Attributes["weight"]);
        }
    }
}

