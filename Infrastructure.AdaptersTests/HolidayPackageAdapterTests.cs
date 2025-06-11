using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.ValueObjects;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Moq;
using Core.Domain.Enums;

namespace Infrastructure.Adapters.Tests
{
    [TestClass()]
    public class HolidayPackageAdapterTests
    {
        private Mock<IHttpClientFactory>? _mockHttpClientFactory;
        private Mock<IExternalProductApiAdapter>? _mockAdapter;
        private IExternalProductApiAdapter? _adapter;
        private ProductDto? _sampleProductDto;
        private List<ProductDto>? _sampleProducts;

        [TestInitialize]
        public void Setup()
        {
            // Setup mock HTTP client factory
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpClient = new Mock<HttpClient>();
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient.Object);

            // Create mock adapter
            _mockAdapter = new Mock<IExternalProductApiAdapter>();
            _adapter = _mockAdapter.Object;

            // Create sample test data
            _sampleProductDto = new ProductDto(
                Guid.NewGuid(),
                "HOLIDAY-123",
                "Beach Resort Package",
                Price.Create(1200, "USD"),
                new AvailabilityInfo { RemainingSlots = 5 },
                "Enjoy a week at our luxury beach resort with all meals included",
                ProductCategory.HolidayPackage,
                "HolidayProvider",
                new List<string> { "https://example.com/holiday1.jpg" },
                DateTime.UtcNow.AddDays(-15),
                DateTime.UtcNow
            );

            // Create sample product list
            _sampleProducts = new List<ProductDto>
            {
                _sampleProductDto,
                new ProductDto(
                    Guid.NewGuid(),
                    "HOLIDAY-456",
                    "Mountain Ski Resort Package",
                    Price.Create(1500, "USD"),
                    new AvailabilityInfo() { RemainingSlots = 8 },
                    "A week of skiing at our premium mountain resort",
                    ProductCategory.HolidayPackage,
                    "HolidayProvider",
                    new List<string> { "https://example.com/holiday2.jpg" },
                    DateTime.UtcNow.AddDays(-8),
                    DateTime.UtcNow
                )
            };
        }

        [TestMethod()]
        public async Task FetchProductsAsyncTest_ReturnsListOfProducts()
        {
            // Arrange
            _mockAdapter!.Setup(a => a.FetchProductsAsync())
                .ReturnsAsync(_sampleProducts!);

            // Act
            var results = await _adapter!.FetchProductsAsync();

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual("HOLIDAY-123", results[0].ExternalId);
            Assert.AreEqual("HOLIDAY-456", results[1].ExternalId);
        }

        [TestMethod()]
        public async Task FetchProductByIdAsyncTest_WhenProductExists_ReturnsProduct()
        {
            // Arrange
            string productId = "HOLIDAY-123";

            // Configure mock to return the sample product
            _mockAdapter!.Setup(a => a.FetchProductByIdAsync(productId))
                .ReturnsAsync(_sampleProductDto);

            // Act
            var result = await _adapter!.FetchProductByIdAsync(productId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productId, result.ExternalId);
            Assert.AreEqual("Beach Resort Package", result.Name);
            Assert.AreEqual(ProductCategory.HolidayPackage, result.Category);
        }

        [TestMethod()]
        public async Task FetchProductByIdAsyncTest_WhenProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            string nonExistentProductId = "NONEXISTENT-ID";

            // Configure mock to return null for non-existent product
            _mockAdapter!.Setup(a => a.FetchProductByIdAsync(nonExistentProductId))
                .ReturnsAsync((ProductDto?)null);

            // Act
            var result = await _adapter!.FetchProductByIdAsync(nonExistentProductId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public async Task PurchaseProductAsyncTest_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            string productId = "HOLIDAY-123";
            int quantity = 1;
            var productDto = _sampleProducts!.First(p => p.ExternalId == productId);

            // Configure mock to return a successful purchase response
            var successResponse = new PurchaseResponseDto(productId) { IsSuccess = true };
            _mockAdapter!.Setup(a => a.PurchaseProductAsync(It.Is<ProductDto>(p => p.ExternalId == productId), quantity))
                .ReturnsAsync(successResponse);

            // Act
            var result = await _adapter!.PurchaseProductAsync(productDto, quantity);

            // Assert
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod()]
        public async Task PurchaseProductAsyncTest_WhenProductNotAvailable_ReturnsFalse()
        {
            // Arrange
            string productId = "HOLIDAY-789";
            int quantity = 2;
            var productDto = new ProductDto(
                Guid.NewGuid(),
                productId,
                "Unavailable Holiday Package",
                Price.Create(2000, "USD"),
                new AvailabilityInfo() { RemainingSlots = 0 },
                "This holiday package is not available",
                ProductCategory.HolidayPackage,
                "HolidayProvider",
                new List<string>(),
                DateTime.UtcNow,
                DateTime.UtcNow
            );

            // Configure mock to return a failed purchase response
            var failedResponse = new PurchaseResponseDto(productId) { IsSuccess = false };
            _mockAdapter!.Setup(a => a.PurchaseProductAsync(It.Is<ProductDto>(p => p.ExternalId == productId), quantity))
                .ReturnsAsync(failedResponse);

            // Act
            var result = await _adapter!.PurchaseProductAsync(productDto, quantity);

            // Assert
            Assert.IsFalse(result.IsSuccess);
        }
    }
}

