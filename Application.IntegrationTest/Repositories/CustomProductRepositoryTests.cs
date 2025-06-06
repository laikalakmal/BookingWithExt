using Application.IntegrationTest.Fixtures;
using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.Repositories.Concreate;
using System.ComponentModel;
namespace Application.IntegrationTest.Repositories
{
    [TestClass]
    public class CustomProductRepositoryTests
    {
        private DatabaseFixture _fixture;
        private CustomProductRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _fixture = new DatabaseFixture();
            _repository = new CustomProductRepository(_fixture.DbContext);
        }

        [TestMethod]
        public async Task AddProductsAsync_AddsProductsToDatabase()
        {
            // Arrange
            var product = new CustomProduct(
                externalId: "CUSTOM-INT-123",
                name: "Integration Test Product",
                price: Price.Create(149.99m, "USD"),
                description: "A product for integration testing",
                category: ProductCategory.Custom,
                provider: "TestProvider",
                availability: new AvailabilityInfo("Available", 10),
                attributes: new Dictionary<string, object> { { "TestKey", "TestValue" } }
            )
            {
                ImageUrl = "https://example.com/test.jpg"
            };

            // Act
            await _repository.AddProductsAsync(new List<CustomProduct> { product });
            await _fixture.DbContext.SaveChangesAsync();

            // Assert
            var savedProduct = await _fixture.DbContext.Products
                .OfType<CustomProduct>()
                .FirstOrDefaultAsync(p => p.ExternalId == "CUSTOM-INT-123");

            Assert.IsNotNull(savedProduct);
            Assert.AreEqual("Integration Test Product", savedProduct.Name);
            Assert.AreEqual(149.99m, savedProduct.Price.Amount);
            Assert.AreEqual("TestProvider", savedProduct.Provider);
            Assert.AreEqual(10, savedProduct.Availability.RemainingSlots);
            Assert.AreEqual("TestValue", savedProduct.Attributes["TestKey"]);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsCorrectProduct()
        {
            // Arrange
            var product = new CustomProduct(
                externalId: "CUSTOM-INT-456",
                name: "GetById Test Product",
                price: Price.Create(199.99m, "USD"),
                description: "A product for GetById testing",
                category: ProductCategory.Custom,
                provider: "TestProvider",
                availability: new AvailabilityInfo("Available", 5),
                attributes: new Dictionary<string, object> { { "Key", "Value" } }
            )
            {
                ImageUrl = "https://example.com/getbyid.jpg"
            };

            _fixture.DbContext.Products.Add(product);
            await _fixture.DbContext.SaveChangesAsync();
            // Act
            var retrievedProduct = await _repository.GetByIdAsync(product.Id);
            // Assert
            Assert.IsNotNull(retrievedProduct);
            Assert.AreEqual(product.Id, retrievedProduct.Id);
            Assert.AreEqual("GetById Test Product", retrievedProduct.Name);
        }

        [TestMethod]
        public async Task GetProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var product1 = new CustomProduct(
                externalId: "CUSTOM-INT-789",
                name: "GetProducts Test Product 1",
                price: Price.Create(99.99m, "USD"),
                description: "A product for GetProducts testing 1",
                category: ProductCategory.Custom,
                provider: "TestProvider1",
                availability: new AvailabilityInfo("Available", 15),
                attributes: new Dictionary<string, object> { { "Key1", "Value1" } }
            );

            var product2 = new CustomProduct(
                externalId: "CUSTOM-INT-101112",
                name: "GetProducts Test Product 2",
                price: Price.Create(199.99m, "USD"),
                description: "A product for GetProducts testing 2",
                category: ProductCategory.Custom,
                provider: "TestProvider2",
                availability: new AvailabilityInfo("Available", 20),
                attributes: new Dictionary<string, object> { { "Key2", "Value2" } }
            );

            _fixture.DbContext.Products.AddRange(product1, product2);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var products = await _repository.GetProductsAsync();

            // Assert
            Assert.IsNotNull(products);
            Assert.AreEqual(2, products.Count());
            Assert.IsTrue(products.Any(p => p.ExternalId == "CUSTOM-INT-789"));
            Assert.IsTrue(products.Any(p => p.ExternalId == "CUSTOM-INT-101112"));
        }

        [TestMethod]
        public async Task UpdateProduct_UpdateExistingProduct()
        {
            // Arrange
            var product = new CustomProduct(
                externalId: "CUSTOM-INT-456",
                name: "GetById Test Product",
                price: Price.Create(199.99m, "USD"),
                description: "A product for GetById testing",
                category: ProductCategory.Custom,
                provider: "TestProvider",
                availability: new AvailabilityInfo("Available", 5),
                attributes: new Dictionary<string, object> { { "Key", "Value" } }
            )
            {
                ImageUrl = "https://example.com/getbyid.jpg"
            };

            _fixture.DbContext.Products.Add(product);
            await _fixture.DbContext.SaveChangesAsync();
            // Act
            var retrievedProduct = await _repository.GetByIdAsync(product.Id);

            retrievedProduct.ExternalId = "BookWithExt";

            bool result = await _repository.UpdateProduct(retrievedProduct);
            await _fixture.DbContext.SaveChangesAsync();
            var newRretrievedProduct = await _repository.GetByIdAsync(product.Id);


            Assert.IsTrue(result);
            Assert.AreEqual("BookWithExt", newRretrievedProduct.ExternalId);
            Assert.AreEqual(product.Id, newRretrievedProduct.Id);

        }

        [TestMethod]
        public async Task DeleteProduct_DeleteExistingProductWithId()
        {
            // Arrange
            var product = new CustomProduct(
                externalId: "CUSTOM-INT-456",
                name: "GetById Test Product",
                price: Price.Create(199.99m, "USD"),
                description: "A product for GetById testing",
                category: ProductCategory.Custom,
                provider: "TestProvider",
                availability: new AvailabilityInfo("Available", 5),
                attributes: new Dictionary<string, object> { { "Key", "Value" } }
            )
            {
                ImageUrl = "https://example.com/getbyid.jpg"
            };

            _fixture.DbContext.Products.Add(product);
            await _fixture.DbContext.SaveChangesAsync();
            // Act
            var res = await _repository.DeleteProductAsync(product.Id);
            Assert.IsTrue(res);
            Assert.ThrowsExceptionAsync<InvalidOperationException>(async
                () => await _repository.GetByIdAsync(product.Id));

        }





        [TestCleanup]
        public void Cleanup()
        {
            _fixture.Dispose();
        }

    }
}