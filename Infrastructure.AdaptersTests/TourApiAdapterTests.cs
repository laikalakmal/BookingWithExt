using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using Core.Domain.ValueObjects;
using Moq;

namespace Infrastructure.Adapters.Tests
{
    [TestClass]
    public class TourApiAdapterTests
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
                "TOUR-123",
                "City Walking Tour",
                Price.Create(50, "USD"),
                new Core.Domain.Entities.SupportClasses.AvailabilityInfo { RemainingSlots = 10 },
                "Explore the city with our expert guides",
                ProductCategory.TourPackage,
                "TourProvider",
                new List<string> { "https://example.com/tour1.jpg" },
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow
            );

            // Create sample product list
            _sampleProducts = new List<ProductDto>
                {
                    _sampleProductDto,
                    new ProductDto(
                        Guid.NewGuid(),
                        "TOUR-456",
                        "Mountain Hiking Adventure",
                        Price.Create(100, "USD"),
                        new AvailabilityInfo() { RemainingSlots = 10 },
                        "Experience the mountains with professional guides",
                        ProductCategory.TourPackage,
                        "TourProvider",
                        new List<string> { "https://example.com/tour2.jpg" },
                        DateTime.UtcNow.AddDays(-5),
                        DateTime.UtcNow
                    )
                };
        }

        [TestMethod]
        public async Task FetchProductByIdAsyncTest_WhenProductExists_ReturnsProduct()
        {
            // Arrange
            string productId = "TOUR-123";

            // Configure mock to return the sample product
            _mockAdapter!.Setup(a => a.FetchProductByIdAsync(productId))
                .ReturnsAsync(_sampleProductDto);

            // Act
            var result = await _adapter!.FetchProductByIdAsync(productId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productId, result.ExternalId);
            Assert.AreEqual("City Walking Tour", result.Name);
            Assert.AreEqual(ProductCategory.TourPackage, result.Category);
        }

        [TestMethod]
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

        [TestMethod]
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
            Assert.AreEqual("TOUR-123", results[0].ExternalId);
            Assert.AreEqual("TOUR-456", results[1].ExternalId);
        }

        [TestMethod]
        public async Task PurchaseProductAsyncTest_WhenSuccessful_ReturnsTrue()
        {
            // Arrange
            string productId = "TOUR-123";
            int quantity = 2;
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

        [TestMethod]
        public async Task PurchaseProductAsyncTest_WhenProductNotAvailable_ReturnsFalse()
        {
            // Arrange
            string productId = "TOUR-789";
            int quantity = 10;
            var productDto = new ProductDto(
                Guid.NewGuid(),
                productId,
                "Unavailable Tour",
                Price.Create(150, "USD"),
                new AvailabilityInfo() { RemainingSlots = 0 },
                "This tour is not available",
                ProductCategory.TourPackage,
                "TourProvider",
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

