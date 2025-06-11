using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using Core.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.Application.Services.Tests
{
    [TestClass()]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _mockRepository;
        private Mock<IExternalProductApiAdapter> _mockAdapter;
        private List<IExternalProductApiAdapter> _adapters;
        private ProductService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockAdapter = new Mock<IExternalProductApiAdapter>();
            _adapters = new List<IExternalProductApiAdapter> { _mockAdapter.Object };
            _service = new ProductService(_mockRepository.Object, _adapters);
        }

        [TestMethod()]
        public void ProductServiceTest()
        {
            // Arrange
            var repository = new Mock<IProductRepository>();
            var adapters = new List<IExternalProductApiAdapter> { new Mock<IExternalProductApiAdapter>().Object };

            // Act
            var service = new ProductService(repository.Object, adapters);

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProductService_NullRepository_ThrowsArgumentNullException()
        {
            // Act
            var service = new ProductService(null, _adapters);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProductService_NullAdapters_ThrowsArgumentNullException()
        {
            // Act
            var service = new ProductService(_mockRepository.Object, null);
        }

        [TestMethod()]
        public async Task GetProductsAsyncTest()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(
                    "ext1",
                    "Product 1",
                     Price.Create(10.0m, Currency.USD.ToString()),
                    "Description 1",
                    ProductCategory.Custom,
                    "Provider 1",
                    new AvailabilityInfo())
                {
                    Id = Guid.NewGuid()
                }
            };

            _mockRepository.Setup(r => r.GetProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Product 1", result.First().Name);
            _mockRepository.Verify(r => r.GetProductsAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task FetchExternalProductsAsyncTest()
        {
            // Arrange
            var productDtos = new List<ProductDto>
            {
                new ProductDto(
                    id: Guid.NewGuid(),
                    externalId : "ext1",
                    name : "External Product 1",
                    price: Price.Create ( 15.0m, Currency.USD.ToString() ),
                    availability:  new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    description: "External Description",
                    category: ProductCategory.Custom,
                    provider : "External Provider",
                    imageUrl: new List<string>(),
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                    )
                {
                    ExternalId = "ext1",
                    Name = "External Product 1",
                    Price = Price.Create ( 15.0m, Currency.USD.ToString() ),
                    Description = "External Description",
                    Category = ProductCategory.Custom,
                    Provider = "External Provider",
                    Availability = new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    Attributes = new Dictionary<string, object>(),

                }
            };

            _mockAdapter.Setup(a => a.FetchProductsAsync()).ReturnsAsync(productDtos);
            _mockAdapter.Setup(a => a.AdapterName).Returns("TestAdapter");

            // Act
            var result = await _service.FetchExternalProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("External Product 1", result.First().Name);
            Assert.AreEqual("ext1", result.First().ExternalId);
            _mockAdapter.Verify(a => a.FetchProductsAsync(), Times.Once);
        }

        [TestMethod()]
        public async Task SyncProductsFromExternalAsyncTest()
        {
            // Arrange
            var externalProducts = new List<ProductDto>
            {

                new ProductDto(
                    id: Guid.NewGuid(),
                    externalId : "ext1",
                    name : "External Product 1",
                    price: Price.Create ( 15.0m, Currency.USD.ToString() ),
                    availability:  new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    description: "External Description",
                    category: ProductCategory.Custom,
                    provider : "External Provider",
                    imageUrl: new List<string>(),
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                    )
                {
                    ExternalId = "ext1",
                    Name = "External Product 1",
                    Price = Price.Create ( 15.0m, Currency.USD.ToString() ),
                    Description = "External Description",
                    Category = ProductCategory.Custom,
                    Provider = "External Provider",
                    Availability = new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    Attributes = new Dictionary<string, object>(),

                }
            };

            _mockAdapter.Setup(a => a.FetchProductsAsync()).ReturnsAsync(externalProducts);
            _mockRepository.Setup(r => r.GetProductsAsync()).ReturnsAsync(new List<Product>());
            _mockRepository.Setup(r => r.AddProductsAsync(It.IsAny<List<Product>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.SyncProductsFromExternalAsync();

            // Assert
            Assert.AreEqual(1, result);
            _mockRepository.Verify(r => r.AddProductsAsync(It.IsAny<List<Product>>()), Times.Once);
        }

        [TestMethod()]
        public void MapToDomainTest()
        {
            // Arrange
            var dto =
                new ProductDto(
                    id: Guid.NewGuid(),
                    externalId: "ext1",
                    name: "External Product 1",
                    price: Price.Create(15.0m, Currency.USD.ToString()),
                    availability: new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    description: "External Description",
                    category: ProductCategory.Custom,
                    provider: "External Provider",
                    imageUrl: new List<string>(),
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                    )
                {
                    ExternalId = "ext1",
                    Name = "External Product 1",
                    Price = Price.Create(15.0m, Currency.USD.ToString()),
                    Description = "External Description",
                    Category = ProductCategory.Custom,
                    Provider = "External Provider",
                    Availability = new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    Attributes = new Dictionary<string, object>(),

                };

            // Act
            var result = _service.MapToDomain(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Id, result.Id);
            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual(dto.Description, result.Description);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapToDomain_NullDto_ThrowsArgumentNullException()
        {
            // Act
            _service.MapToDomain(null);
        }

        [TestMethod()]
        public void MapToDtoTest()
        {
            // Arrange
            var product = new Product(
                    "ext1",
                    "Product 1",
                     Price.Create(10.0m, Currency.USD.ToString()),
                    "Description 1",
                    ProductCategory.Custom,
                    "Provider 1",
                    new AvailabilityInfo())
            {
                Id = Guid.NewGuid()
            };

            // Act
            var result = _service.MapToDto(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.Id, result.Id);
            Assert.AreEqual(product.Name, result.Name);
            Assert.AreEqual(product.Description, result.Description);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MapToDto_NullProduct_ThrowsArgumentNullException()
        {
            // Act
            _service.MapToDto(null);
        }

        [TestMethod()]
        public async Task GetByIdAsyncTest()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product(
                    "ext1",
                    "Product 1",
                     Price.Create(10.0m, Currency.USD.ToString()),
                    "Description 1",
                    ProductCategory.Custom,
                    "Provider 1",
                    new AvailabilityInfo())
            {
                Id = productId
            };


            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productId, result.Id);
            Assert.AreEqual("Product 1", result.Name);
            _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task GetByIdAsync_ProductNotFound_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            await _service.GetByIdAsync(productId);
        }

        [TestMethod()]
        public async Task PurchaseProductAsyncTest()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product(
                    "ext1",
                    "Product 1",
                     Price.Create(10.0m, Currency.USD.ToString()),
                    "Description 1",
                    ProductCategory.Custom,
                    "Provider 1",
                    new AvailabilityInfo(){RemainingSlots= 20})
            {
                Id = productId
            };

            var productDto =
                new ProductDto(
                    id: productId,
                    externalId: "ext1",
                    name: "External Product 1",
                    price: Price.Create(15.0m, Currency.USD.ToString()),
                    availability: new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    description: "External Description",
                    category: ProductCategory.Custom,
                    provider: "External Provider",
                    imageUrl: new List<string>(),
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                    )
                {
                   
                    Attributes = new Dictionary<string, object>(),

                };

            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(true);

            // Act
            var result = await _service.PurchaseProductAsync(productDto, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(productId, result.ProductId);
            Assert.AreEqual(2, result.Quantity);

            _mockRepository.Verify(r => r.UpdateProduct(It.IsAny<Product>()), Times.Once);
            Assert.AreEqual(18, product.Availability.RemainingSlots);
        }

        [TestMethod()]
        [ExpectedException(typeof(Exception))]
        public async Task PurchaseProductAsync_InvalidQuantity_ThrowsException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product(
                "ext1",
                "Product 1",
                 Price.Create(10.0m, Currency.USD.ToString()),
                "Description 1",
                ProductCategory.Custom,
                "Provider 1",
                new AvailabilityInfo()
                {
                    IsAvailable = true,
                    RemainingSlots = 5,
                    Status = "limited"
                })
            {
                Id = Guid.NewGuid()
            };


            var productDto =
                new ProductDto(
                    id: productId,
                    externalId: "ext1",
                    name: "External Product 1",
                    price: Price.Create(15.0m, Currency.USD.ToString()),
                    availability: new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    description: "External Description",
                    category: ProductCategory.Custom,
                    provider: "External Provider",
                    imageUrl: new List<string>(),
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                    )
                {
                    ExternalId = "ext1",
                    Name = "Product 1",
                    Price = Price.Create(15.0m, Currency.USD.ToString()),
                    Description = "External Description",
                    Category = ProductCategory.Custom,
                    Provider = "External Provider",
                    Availability = new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    Attributes = new Dictionary<string, object>(),

                };

            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act - Try to purchase more than available
            await _service.PurchaseProductAsync(productDto, 10);
        }

        [TestMethod()]
        public async Task DeleteProductAsyncTest()
        {
            // Arrange
            var productId = Guid.NewGuid();
           var product= new Product(
                "ext1",
                "Product 1",
                 Price.Create(10.0m, Currency.USD.ToString()),
                "Description 1",
                ProductCategory.Custom,
                "Provider 1",
                new AvailabilityInfo())
            {
                Id = productId
            };

            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.DeleteProductAsync(product)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteProductAsync(productId);

            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(r => r.DeleteProductAsync(product), Times.Once);
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeleteProductAsync_ProductNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            await _service.DeleteProductAsync(productId);
        }

        [TestMethod()]
        public async Task EditProductTest()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Product(
                "ext1",
                "Product 1",
                 Price.Create(10.0m, Currency.USD.ToString()),
                "Description 1",
                ProductCategory.Custom,
                "Provider 1",
                new AvailabilityInfo())
            {
                Id = productId
            };

            var updatedDto =
                new ProductDto(
                    id: productId,
                    externalId: "ext1",
                    name: "updated Product",
                    price: Price.Create(15.0m, Currency.USD.ToString()),
                    availability: new AvailabilityInfo { IsAvailable = true, RemainingSlots = 10 },
                    description: "updated Description",
                    category: ProductCategory.HolidayPackage,
                    provider: "updated Provider",
                    imageUrl: new List<string>(),
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                    )
                {
                    Attributes = new Dictionary<string, object>(),

                };

            _mockRepository.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(existingProduct);
            _mockRepository.Setup(r => r.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(true);

            // Act
            var result = await _service.EditProduct(productId, updatedDto);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("Updated Product", existingProduct.Name);
            Assert.AreEqual("Updated Description", existingProduct.Description);
            Assert.AreEqual(ProductCategory.HolidayPackage, existingProduct.Category);
            _mockRepository.Verify(r => r.UpdateProduct(existingProduct), Times.Once);
        }

        

        [TestMethod()]
        public async Task AddProductAsyncTest()
        {
            // Arrange
            var productDto =
                new ProductDto(
                    id: Guid.NewGuid(),
                    externalId: "ext1",
                    name: "External Product 1",
                    price: Price.Create(15.0m, Currency.USD.ToString()),
                    availability: new AvailabilityInfo { IsAvailable = true, RemainingSlots = 20 },
                    description: "External Description",
                    category: ProductCategory.Custom,
                    provider: "External Provider",
                    imageUrl: new List<string>(),
                    createdAt: DateTime.UtcNow,
                    updatedAt: DateTime.UtcNow
                    )
                {
                   
                    Attributes = new Dictionary<string, object>(),

                };

            _mockRepository.Setup(r => r.AddProductsAsync(It.IsAny<List<Product>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddProductAsync(productDto);

            // Assert
            Assert.AreNotEqual(Guid.Empty, result);
            _mockRepository.Verify(r => r.AddProductsAsync(It.IsAny<List<Product>>()), Times.Once);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task AddProductAsync_NullProduct_ThrowsArgumentNullException()
        {
            // Act
            await _service.AddProductAsync(null);
        }
    }
}