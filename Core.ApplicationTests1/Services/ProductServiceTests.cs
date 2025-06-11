using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Services.Tests
{
    [TestClass()]
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _mockRepository;
        private Mock<IExternalProductApiAdapter> _mockAdapter;
        private ProductService _service;

        [TestInitialize]
        public void Initialize()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockAdapter = new Mock<IExternalProductApiAdapter>();
            _mockAdapter.Setup(a => a.AdapterName).Returns("TestAdapter");
            
            var adapters = new List<IExternalProductApiAdapter> { _mockAdapter.Object };
            _service = new ProductService(_mockRepository.Object, adapters);
        }

        [TestMethod()]
        public void ProductServiceTest()
        {
            // Test constructor with valid parameters
            var adapters = new List<IExternalProductApiAdapter> { _mockAdapter.Object };
            var service = new ProductService(_mockRepository.Object, adapters);
            Assert.IsNotNull(service);

            // Test constructor with null repository
            Assert.ThrowsException<ArgumentNullException>(() => 
                new ProductService(null, adapters));

            // Test constructor with null adapters
            Assert.ThrowsException<ArgumentNullException>(() => 
                new ProductService(_mockRepository.Object, null));
        }

        [TestMethod()]
        public async Task GetProductsAsyncTest()
        {
            // Test when products exist
            var products = CreateTestProducts(3);
            _mockRepository.Setup(r => r.GetProductsAsync()).ReturnsAsync(products);
            
            var result = await _service.GetProductsAsync();
            
            Assert.AreEqual(3, result.Count());
            _mockRepository.Verify(r => r.GetProductsAsync(), Times.Once);

            // Test when no products exist
            _mockRepository.Setup(r => r.GetProductsAsync()).ReturnsAsync((IEnumerable<Product>)null);
            
            var emptyResult = await _service.GetProductsAsync();
            
            Assert.AreEqual(0, emptyResult.Count());

            // Test exception handling
            _mockRepository.Setup(r => r.GetProductsAsync()).ThrowsAsync(new Exception("Test exception"));
            
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _service.GetProductsAsync());
            Assert.IsTrue(ex.Message.Contains("Failed to fetch custom products"));
        }

        [TestMethod()]
        public async Task FetchExternalProductsAsyncTest()
        {
            // Test when adapters return products
            var productDtos = CreateTestProductDtos(2);
            _mockAdapter.Setup(a => a.FetchProductsAsync()).ReturnsAsync(productDtos);
            
            var result = await _service.FetchExternalProductsAsync();
            
            Assert.AreEqual(2, result.Count());
            _mockAdapter.Verify(a => a.FetchProductsAsync(), Times.Once);

            // Test when adapter throws exception
            _mockAdapter.Setup(a => a.FetchProductsAsync()).ThrowsAsync(new Exception("Adapter error"));
            
            var emptyResult = await _service.FetchExternalProductsAsync();
            
            Assert.AreEqual(0, emptyResult.Count());

            // Test with null adapter
            var mockNullAdapter = new Mock<IExternalProductApiAdapter>();
            var adapters = new List<IExternalProductApiAdapter> { null, _mockAdapter.Object };
            var serviceWithNullAdapter = new ProductService(_mockRepository.Object, adapters);
            
            _mockAdapter.Setup(a => a.FetchProductsAsync()).ReturnsAsync(productDtos);
            
            var resultWithNullAdapter = await serviceWithNullAdapter.FetchExternalProductsAsync();
            
            Assert.AreEqual(2, resultWithNullAdapter.Count());
        }

        [TestMethod()]
        public async Task SyncProductsFromExternalAsyncTest()
        {
            // Test when no external products
            var mockService = new Mock<ProductService>(_mockRepository.Object, new[] { _mockAdapter.Object });
            mockService.Setup(s => s.FetchExternalProductsAsync()).ReturnsAsync(new List<Product>());
            
            var result = await mockService.Object.SyncProductsFromExternalAsync();
            
            Assert.AreEqual(0, result);

            // Test adding new products
            var externalProducts = CreateTestProducts(2);
            foreach (var product in externalProducts)
            {
                product.ExternalId = $"ext-{Guid.NewGuid()}";
            }
            
            mockService = new Mock<ProductService>(_mockRepository.Object, new[] { _mockAdapter.Object }) { CallBase = true };
            mockService.Setup(s => s.FetchExternalProductsAsync()).ReturnsAsync(externalProducts);
            _mockRepository.Setup(r => r.GetProductsAsync()).ReturnsAsync(new List<Product>());
            
            var addResult = await mockService.Object.SyncProductsFromExternalAsync();
            
            Assert.AreEqual(2, addResult);
            _mockRepository.Verify(r => r.AddProductsAsync(It.Is<List<Product>>(p => p.Count == 2)), Times.Once);

            // Test updating existing products
            var existingProduct = CreateTestProduct();
            existingProduct.ExternalId = "existing-id";
            
            var externalProduct = CreateTestProduct();
            externalProduct.ExternalId = "existing-id";
            externalProduct.Name = "Updated Name";
            
            mockService = new Mock<ProductService>(_mockRepository.Object, new[] { _mockAdapter.Object }) { CallBase = true };
            mockService.Setup(s => s.FetchExternalProductsAsync()).ReturnsAsync(new List<Product> { externalProduct });
            _mockRepository.Setup(r => r.GetProductsAsync()).ReturnsAsync(new List<Product> { existingProduct });
            _mockRepository.Setup(r => r.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(true);
            
            var updateResult = await mockService.Object.SyncProductsFromExternalAsync();
            
            Assert.AreEqual(1, updateResult);
            _mockRepository.Verify(r => r.UpdateProduct(It.Is<Product>(p => p.Name == "Updated Name")), Times.Once);
        }

        [TestMethod()]
        public void MapToDomainTest()
        {
            // Test valid conversion
            var dto = CreateTestProductDto();
            
            var result = _service.MapToDomain(dto);
            
            Assert.AreEqual(dto.Id, result.Id);
            Assert.AreEqual(dto.Name, result.Name);
            Assert.AreEqual(dto.Description, result.Description);

            // Test null argument
            Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDomain(null));
        }

        [TestMethod()]
        public void MapToDtoTest()
        {
            // Test valid conversion
            var product = CreateTestProduct();
            
            var result = _service.MapToDto(product);
            
            Assert.AreEqual(product.Id, result.Id);
            Assert.AreEqual(product.Name, result.Name);
            Assert.AreEqual(product.Description, result.Description);

            // Test null argument
            Assert.ThrowsException<ArgumentNullException>(() => _service.MapToDto(null));
        }

        [TestMethod()]
        public async Task GetByIdAsyncTest()
        {
            // Test when product exists
            var product = CreateTestProduct();
            _mockRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            
            var result = await _service.GetByIdAsync(product.Id);
            
            Assert.AreEqual(product.Id, result.Id);
            Assert.AreEqual(product.Name, result.Name);

            // Test when product doesn't exist
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product)null);
            
            var ex = await Assert.ThrowsExceptionAsync<Exception>(() => _service.GetByIdAsync(id));
            Assert.IsTrue(ex.Message.Contains($"Product with ID {id} not found"));
        }

        [TestMethod()]
        public async Task PurchaseProductAsyncTest()
        {
            // Test successful purchase
            var product = CreateTestProduct();
            product.Availability.RemainingSlots = 10;
            
            var productDto = CreateTestProductDto();
            productDto.Id = product.Id;
            productDto.Availability.RemainingSlots = 10;
            
            _mockRepository.Setup(r => r.GetByIdAsync(productDto.Id)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(true);
            
            var result = await _service.PurchaseProductAsync(productDto, 2);
            
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(productDto.Id, result.ProductId);
            Assert.AreEqual(2, result.Quantity);
            Assert.AreEqual(productDto.Price.Amount * 2, result.TotalAmount);
            _mockRepository.Verify(r => r.UpdateProduct(It.Is<Product>(p => p.Availability.RemainingSlots == 8)), Times.Once);

            // Test invalid quantity
            var ex1 = await Assert.ThrowsExceptionAsync<Exception>(
                () => _service.PurchaseProductAsync(productDto, 20));
            Assert.IsTrue(ex1.Message.Contains("Invalid quantity"));

            // Test product not found
            var nonExistingProductDto = CreateTestProductDto();
            _mockRepository.Setup(r => r.GetByIdAsync(nonExistingProductDto.Id)).ReturnsAsync((Product)null);
            
            var ex2 = await Assert.ThrowsExceptionAsync<Exception>(
                () => _service.PurchaseProductAsync(nonExistingProductDto, 1));
            Assert.IsTrue(ex2.Message.Contains("not found"));
        }

        [TestMethod()]
        public async Task DeleteProductAsyncTest()
        {
            // Test successful deletion
            var product = CreateTestProduct();
            _mockRepository.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);
            _mockRepository.Setup(r => r.DeleteProductAsync(product)).ReturnsAsync(true);
            
            var result = await _service.DeleteProductAsync(product.Id);
            
            Assert.IsTrue(result);
            _mockRepository.Verify(r => r.DeleteProductAsync(product), Times.Once);

            // Test product not found
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product)null);
            
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _service.DeleteProductAsync(id));
        }

        [TestMethod()]
        public async Task EditProductTest()
        {
            // Test successful edit
            var existingProduct = CreateTestProduct();
            var updatedProductDto = CreateTestProductDto();
            updatedProductDto.Name = "Updated Name";
            
            _mockRepository.Setup(r => r.GetByIdAsync(existingProduct.Id)).ReturnsAsync(existingProduct);
            _mockRepository.Setup(r => r.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(true);
            
            var result = await _service.EditProduct(existingProduct.Id, updatedProductDto);
            
            Assert.IsTrue(result);
            _mockRepository.Verify(r => r.UpdateProduct(It.Is<Product>(p => p.Name == "Updated Name")), Times.Once);

            // Test product not found
            var id = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Product)null);
            
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(() => _service.EditProduct(id, updatedProductDto));

            // Test null DTO
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _service.EditProduct(existingProduct.Id, null));
        }

        [TestMethod()]
        public async Task AddProductAsyncTest()
        {
            // Test successful add
            var productDto = CreateTestProductDto();
            _mockRepository.Setup(r => r.AddProductsAsync(It.IsAny<List<Product>>())).Returns(Task.CompletedTask);
            
            var result = await _service.AddProductAsync(productDto);
            
            Assert.AreNotEqual(Guid.Empty, result);
            _mockRepository.Verify(r => r.AddProductsAsync(It.Is<List<Product>>(list => list.Count == 1)), Times.Once);

            // Test null DTO
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _service.AddProductAsync(null));
        }

        #region Helper Methods
        
        private List<Product> CreateTestProducts(int count)
        {
            var products = new List<Product>();
            for (int i = 0; i < count; i++)
            {
                products.Add(CreateTestProduct(i));
            }
            return products;
        }

        private Product CreateTestProduct(int index = 0)
        {
            return new Product(
                externalId: $"ext-{index}",
                name: $"Product {index}",
                price: Price.Create(100 + index, "USD"),
                description: $"Description {index}",
                category: Domain.Enums.ProductCategory.Custom,
                provider: "TestProvider",
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 10)
            )
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ImageUrl = new List<string> { $"image-{index}.jpg" },
                Attributes = new Dictionary<string, object>()
            };
        }

        private List<ProductDto> CreateTestProductDtos(int count)
        {
            var dtos = new List<ProductDto>();
            for (int i = 0; i < count; i++)
            {
                dtos.Add(CreateTestProductDto(i));
            }
            return dtos;
        }

        private ProductDto CreateTestProductDto(int index = 0)
        {
            return new ProductDto(
                id: Guid.NewGuid(),
                externalId: $"ext-{index}",
                name: $"Product {index}",
                price: Price.Create(100 + index, "USD"),
                availability: new Domain.Entities.SupportClasses.AvailabilityInfo("Available", 10),
                description: $"Description {index}",
                category: Domain.Enums.ProductCategory.Custom,
                provider: "TestProvider",
                imageUrl: new List<string> { $"image-{index}.jpg" },
                createdAt: DateTime.UtcNow,
                updatedAt: DateTime.UtcNow
            )
            {
                Attributes = new Dictionary<string, object>()
            };
        }
        
        #endregion
    }
}