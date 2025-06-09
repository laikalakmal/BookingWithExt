using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Moq;

namespace Core.Application.Services.Concreate.Tests
{
    [TestClass()]
    public class CustomProductServiceTests
    {
        private Mock<IProductRepository<CustomProduct>> _mockRepository;
        private ProductService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IProductRepository<CustomProduct>>();
            _service = new ProductService(_mockRepository.Object);
        }

        [TestMethod()]
        public void CustomProductServiceTest()
        {
            // Arrange & Act
            var service = new ProductService(_mockRepository.Object);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod()]
        public async Task GetProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var testProducts = new List<CustomProduct>
            {
                new CustomProduct(
                    externalId: "CUSTOM123",
                    name: "Custom Experience",
                    price: Price.Create(199.99m, "USD"),
                    description: "A unique custom experience",
                    category: ProductCategory.Custom,
                    provider: "LocalProvider",
                    availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                    attributes: new Dictionary<string, object> { { "Location", "City Center" } }
                )
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = "image.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _mockRepository.Setup(repo => repo.GetProductsAsync())
                .ReturnsAsync(testProducts);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());

            var dto = result.First();
            Assert.AreEqual("CUSTOM123", dto.ExternalId);
            Assert.AreEqual("Custom Experience", dto.Name);
            Assert.AreEqual("City Center", dto.Attributes["Location"]);
        }

        [TestMethod()]
        public async Task GetProductsAsync_HandlesNullResult()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetProductsAsync())
                .ReturnsAsync((IEnumerable<CustomProduct>)null);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());
        }

        [TestMethod()]
        public async Task GetProductsAsync_HandlesException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetProductsAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _service.GetProductsAsync());
            Assert.IsTrue(ex.Message.Contains("Failed to fetch custom products"));
        }

        [TestMethod()]
        public async Task FetchExternalProductsAsyncTest()
        {
            // Arrange & Act
            var result = await _service.FetchExternalProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any(), "Should return empty collection as custom products don't have external sources");
        }

        [TestMethod()]
        public async Task SyncProductsFromExternalAsyncTest()
        {
            // Arrange & Act
            var result = await _service.SyncProductsFromExternalAsync();

            // Assert
            Assert.AreEqual(0, result, "Should return 0 as custom products don't sync from external sources");
        }

        [TestMethod()]
        public void MapToDomain_ValidDto_ReturnsCustomProduct()
        {
            // Arrange
            var dto = new CustomProductDto(
                id: Guid.NewGuid(),
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                imageUrl: "image.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                attributes: new Dictionary<string, object> { { "Location", "City Center" } }
            );

            // Act
            var result = _service.MapToDomain(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual(dto.ExternalId, result.ExternalId);
            Assert.AreEqual(dto.Price.Amount, result.Price.Amount);
            Assert.AreEqual(dto.Attributes["Location"], result.Attributes["Location"]);
        }

        [TestMethod()]
        public void MapToDomain_NullDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDomain(null));
        }

        [TestMethod()]
        public void MapToDto_ValidProduct_ReturnsCustomProductDto()
        {
            // Arrange
            var product = new CustomProduct(
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                attributes: new Dictionary<string, object> { { "Location", "City Center" } }
            )
            {
                Id = Guid.NewGuid(),
                ImageUrl = "image.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = _service.MapToDto(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Name, result.Name);
            Assert.AreEqual(product.ExternalId, result.ExternalId);
            Assert.AreEqual(product.Price.Amount, result.Price.Amount);
            Assert.AreEqual(product.Attributes["Location"], result.Attributes["Location"]);
        }

        [TestMethod()]
        public void MapToDto_NullProduct_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(null));
        }

        [TestMethod()]
        public async Task GetByIdAsync_ExistingId_ReturnsProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new CustomProduct(
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                attributes: new Dictionary<string, object> { { "Location", "City Center" } }
            )
            {
                Id = productId,
                ImageUrl = "image.jpg",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Id, result.Id);
            Assert.AreEqual(product.Name, result.Name);
        }

        [TestMethod()]
        public async Task GetByIdAsync_NonExistentId_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((CustomProduct)null);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _service.GetByIdAsync(productId));
            Assert.IsTrue(ex.Message.Contains("not found"));
        }

        [TestMethod()]
        public async Task PurchaseProductAsync_ValidPurchase_ReturnsSuccessResponse()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDto = new ProductDto(
                id: productId,
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                imageUrl: "image.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow
            );

            var customProduct = new CustomProduct(
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                attributes: new Dictionary<string, object>()
            )
            {
                Id = productId
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(customProduct);

            _mockRepository.Setup(repo => repo.UpdateProduct(It.IsAny<CustomProduct>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.PurchaseProductAsync(productDto, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(productId, result.ProductId);
            Assert.AreEqual(2, result.Quantity);
            Assert.AreEqual(399.98m, result.TotalAmount);
            Assert.AreEqual("USD", result.CurrencyCode);
            Assert.AreEqual("LocalProvider", result.Provider);

            // Verify inventory was updated
            _mockRepository.Verify(repo => repo.UpdateProduct(It.Is<CustomProduct>(p =>
                p.Id == productId && p.Availability.RemainingSlots == 13)), Times.Once);
        }

        [TestMethod()]
        public async Task PurchaseProductAsync_ProductNotFound_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDto = new ProductDto(
                id: productId,
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                imageUrl: "image.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow
            );

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((CustomProduct)null);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.PurchaseProductAsync(productDto, 2));
            Assert.IsTrue(ex.Message.Contains("not found"));
        }

        [TestMethod()]
        public async Task PurchaseProductAsync_InvalidQuantity_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDto = new ProductDto(
                id: productId,
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 5),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                imageUrl: "image.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow
            );

            var customProduct = new CustomProduct(
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 5),
                attributes: new Dictionary<string, object>()
            )
            {
                Id = productId
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(customProduct);

            // Act & Assert - Try to purchase more than available
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.PurchaseProductAsync(productDto, 10));
            Assert.IsTrue(ex.Message.Contains("Invalid quantity"));
        }

        [TestMethod()]
        public async Task DeleteProductAsync_ExistingProduct_ReturnsTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new CustomProduct(
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                attributes: new Dictionary<string, object>()
            )
            {
                Id = productId
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(product);
            _mockRepository.Setup(repo => repo.DeleteProductAsync(product))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProductAsync(productId);

            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(repo => repo.DeleteProductAsync(product), Times.Once);
        }

        [TestMethod()]
        public async Task DeleteProductAsync_NonExistentProduct_ThrowsKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((CustomProduct)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() =>
                _service.DeleteProductAsync(productId));
        }

        [TestMethod()]
        public async Task EditProduct_ValidUpdate_ReturnsTrue()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new CustomProduct(
                externalId: "CUSTOM123",
                name: "Custom Experience",
                price: Price.Create(199.99m, "USD"),
                description: "A unique custom experience",
                category: ProductCategory.Custom,
                provider: "LocalProvider",
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                attributes: new Dictionary<string, object>()
            )
            {
                Id = productId,
                ImageUrl = "old-image.jpg"
            };

            var updatedDto = new CustomProductDto(
                id: productId,
                externalId: "CUSTOM123",
                name: "Updated Custom Experience",
                price: Price.Create(249.99m, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 10),
                description: "An updated unique custom experience",
                category: ProductCategory.Custom,
                provider: "UpdatedProvider",
                imageUrl: "new-image.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                attributes: new Dictionary<string, object> { { "Location", "Downtown" } }
            );

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);
            _mockRepository.Setup(repo => repo.UpdateProduct(It.IsAny<CustomProduct>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.EditProduct(productId, updatedDto);

            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(repo => repo.UpdateProduct(It.Is<CustomProduct>(p =>
                p.Id == productId &&
                p.Name == "Updated Custom Experience" &&
                p.Price.Amount == 249.99m &&
                p.Provider == "UpdatedProvider" &&
                p.ImageUrl == "new-image.jpg")),
                Times.Once);
        }

        [TestMethod()]
        public async Task EditProduct_NonExistentProduct_ThrowsKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updatedDto = new CustomProductDto(
                id: productId,
                externalId: "CUSTOM123",
                name: "Updated Custom Experience",
                price: Price.Create(249.99m, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 10),
                description: "An updated unique custom experience",
                category: ProductCategory.Custom,
                provider: "UpdatedProvider",
                imageUrl: "new-image.jpg",
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow,
                attributes: new Dictionary<string, object>()
            );

            _mockRepository.Setup(repo => repo.GetByIdAsync(productId))
                .ReturnsAsync((CustomProduct)null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.EditProduct(productId, updatedDto));
        }

        [TestMethod()]
        public async Task AddProductAsync_ValidProduct_ReturnsNewId()
        {
            // Arrange
            var productDto = new CustomProductDto(
                id: Guid.Empty,
                externalId: "CUSTOM123",
                name: "New Custom Experience",
                price: Price.Create(199.99m, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 15),
                description: "A brand new custom experience",
                category: Domain.Enums.ProductCategory.Custom,
                provider: "LocalProvider",
                imageUrl: "image.jpg",
                createdAt: DateTime.MinValue,
                updatedAt: DateTime.MinValue,
                attributes: new Dictionary<string, object> { { "Location", "Beachfront" } }
            );

            _mockRepository.Setup(repo => repo.AddProductsAsync(It.IsAny<List<CustomProduct>>()))
                .Returns(Task.CompletedTask);

            // Act
            var newId = await _service.AddProductAsync(productDto);

            // Assert
            Assert.AreNotEqual(Guid.Empty, newId);
            _mockRepository.Verify(repo => repo.AddProductsAsync(It.Is<List<CustomProduct>>(list =>
                list.Count == 1 &&
                list[0].Name == "New Custom Experience" &&
                list[0].Attributes.ContainsKey("Location"))),
                Times.Once);
        }

        [TestMethod()]
        public async Task AddProductAsync_NullDto_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                _service.AddProductAsync(null));
        }
    }
}

