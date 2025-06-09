using Application.IntegrationTest.Fixtures;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using Infrastructure.Persistence.Repositories;

namespace Application.IntegrationTest.Services
{
    [TestClass]
    public class CustomProductServiceIntegrationTests
    {
        private DatabaseFixture _fixture;
        private IProductRepository<CustomProduct> _repository;
        private Core.Application.Services.ProductService _service;

        private Guid? TestProductId { get; set; }

        [TestInitialize]
        public void Setup()
        {
            _fixture = new DatabaseFixture();
            _repository = new CustomProductRepository(_fixture.DbContext);
            _service = new CustomProductService(_repository);

            // Seed test data
            SeedTestData().Wait();
        }

        private async Task SeedTestData()
        {
            var product = new CustomProduct(
                externalId: "CUSTOM-SVC-123",
                name: "Service Integration Test Product",
                price: Price.Create(199.99m, "USD"),
                description: "A product for service integration testing",
                category: ProductCategory.Custom,
                provider: "IntegrationTestProvider",
                availability: new AvailabilityInfo("Available", 20),
                attributes: new Dictionary<string, object> { { "Feature", "Premium" } }
            )
            {
                ImageUrl = "https://example.com/integration.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };

            TestProductId = product.Id;

            _fixture.DbContext.Products.Add(product);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _fixture.Dispose();
        }

        [TestMethod]
        public async Task GetProductsAsync_ReturnsAllProducts()
        {
            // Act
            var products = await _service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(products);
            Assert.IsTrue(products.Any());

            var product = products.FirstOrDefault(p => p.ExternalId == "CUSTOM-SVC-123");
            Assert.IsNotNull(product);
            Assert.AreEqual("Service Integration Test Product", product.Name);
            Assert.AreEqual(199.99m, product.Price.Amount);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsProductById()
        {
            // Arrange
            var product = await _repository.GetByIdAsync(TestProductId.Value);
            // Act
            var result = await _service.GetByIdAsync(product.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Service Integration Test Product", result.Name);
            Assert.AreEqual(199.99m, result.Price.Amount);
        }

        [TestMethod]
        public async Task PurchaseProductAsync_UpdatesAvailabilityAndReturnsSuccessResponse()
        {
            // Arrange
            var product = await _repository.GetByIdAsync(TestProductId.Value);
            var purchaseDto = new CustomProductDto(
                id: product.Id,
                externalId: product.ExternalId,
                name: product.Name,
                price: product.Price,
                availability: product.Availability,
                description: product.Description,
                category: product.Category,
                provider: product.Provider,
                imageUrl: product.ImageUrl,
                createdAt: product.CreatedAt,
                updatedAt: product.UpdatedAt,
                attributes: product.Attributes
            );

            // Act
            var response = await _service.PurchaseProductAsync(purchaseDto, 2);

            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(2, response.Quantity);
            Assert.AreEqual(18, product.Availability.RemainingSlots);
        }

        [TestMethod]
        public async Task PurchaseProductAsync_InvalidQuantity_ThrowsException()
        {
            // Arrange
            var product = await _repository.GetByIdAsync(TestProductId.Value);
            var purchaseDto = new CustomProductDto(
                id: product.Id,
                externalId: product.ExternalId,
                name: product.Name,
                price: product.Price,
                availability: product.Availability,
                description: product.Description,
                category: product.Category,
                provider: product.Provider,
                imageUrl: product.ImageUrl,
                createdAt: product.CreatedAt,
                updatedAt: product.UpdatedAt,
                attributes: product.Attributes
            );

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _service.PurchaseProductAsync(purchaseDto, 25));
        }

        [TestMethod]
        public async Task MapToDomain_ValidDto_ReturnsCustomProduct()
        {
            // Arrange
            var dto = new CustomProductDto(
                id: Guid.NewGuid(),
                externalId: "TEST-EXTERNAL-ID",
                name: "Test Product",
                price: Price.Create(99.99m, "USD"),
                availability: new AvailabilityInfo("Available", 10),
                description: "Test Description",
                category: ProductCategory.Custom,
                provider: "Test Provider",
                imageUrl: "https://example.com/test.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                attributes: new Dictionary<string, object> { { "Key", "Value" } }
            );
            // Act
            var product = _service.MapToDomain(dto);
            // Assert
            Assert.IsNotNull(product);
            Assert.AreEqual(dto.ExternalId, product.ExternalId);
            Assert.AreEqual(dto.Name, product.Name);
            Assert.AreEqual(dto.Price.Amount, product.Price.Amount);
            Assert.AreEqual(dto.Availability.RemainingSlots, product.Availability.RemainingSlots);
            Assert.AreEqual(dto.Attributes["Key"], product.Attributes["Key"]);
        }

        [TestMethod]
        public void MapToDomain_NullDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDomain(null));
        }

        [TestMethod]
        public void MapToDto_ValidProduct_ReturnsCustomProductDto()
        {
            // Arrange
            var product = new CustomProduct(
                externalId: "TEST-EXTERNAL-ID",
                name: "Test Product",
                price: Price.Create(99.99m, "USD"),
                description: "Test Description",
                category: ProductCategory.Custom,
                provider: "Test Provider",
                availability: new AvailabilityInfo("Available", 10),
                attributes: new Dictionary<string, object> { { "Key", "Value" } }
            )
            {
                ImageUrl = "https://example.com/test.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            // Act
            var dto = _service.MapToDto(product);
            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(product.ExternalId, dto.ExternalId);
            Assert.AreEqual(product.Name, dto.Name);
            Assert.AreEqual(product.Price.Amount, dto.Price.Amount);
            Assert.AreEqual(product.Availability.RemainingSlots, dto.Availability.RemainingSlots);
            Assert.AreEqual(product.Attributes["Key"], dto.Attributes["Key"]);
        }

        [TestMethod]
        public void MapToDto_NullProduct_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(null));
        }

    }
}