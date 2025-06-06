using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Concreate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Application.IntegrationTest.Factories
{
    [TestClass]
    public class ProductRepositoryFactoryTests
    {
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void Setup()
        {
            // Create a service collection
            var services = new ServiceCollection();

            // Register in-memory DbContext
            services.AddDbContext<AppDbContext>(options => 
                options.UseInMemoryDatabase("ProductRepositoryFactoryTests"));

            // Register the actual repositories
            services.AddScoped<GenericProductRepository>();
            services.AddScoped<CustomProductRepository>();
            services.AddScoped<TourPackageRepository>();
            services.AddScoped<HolidayPackageRepository>();

            // Build the service provider
            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// A test implementation of IProductRepositoryFactory for CustomProducts
        /// </summary>
        private class TestCustomProductRepositoryFactory : IProductRepositoryFactory
        {
            private readonly IServiceProvider _serviceProvider;

            public TestCustomProductRepositoryFactory(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public bool CanHandle(Product product)
            {
                return product is CustomProduct;
            }

            public bool CanHandle(ProductCategory category)
            {
                return category == ProductCategory.Custom;
            }

            public IProductRepository<Product> CreateRepository()
            {
                // Get the typed repository and adapt it
                var repository = _serviceProvider.GetRequiredService<CustomProductRepository>();
                return new ProductRepositoryAdapter<CustomProduct>(repository);
            }
        }

        // Repository adapter to convert typed repositories to base type
        private class ProductRepositoryAdapter<T> : IProductRepository<Product> where T : Product
        {
            private readonly IProductRepository<T> _repository;

            public ProductRepositoryAdapter(IProductRepository<T> repository)
            {
                _repository = repository;
            }

            public async Task AddProductsAsync(List<Product> products)
            {
                var typedProducts = products.OfType<T>().ToList();
                await _repository.AddProductsAsync(typedProducts);
            }

            public async Task<bool> DeleteProductAsync(Guid id)
            {
                return await _repository.DeleteProductAsync(id);
            }

            public async Task<bool> DeleteProductAsync(Product product)
            {
                if (product is T typedProduct)
                {
                    return await _repository.DeleteProductAsync(typedProduct);
                }
                return false;
            }

            public async Task<Product> GetByIdAsync(Guid id)
            {
                return await _repository.GetByIdAsync(id);
            }

            public async Task<IEnumerable<Product>> GetProductsAsync()
            {
                var products = await _repository.GetProductsAsync();
                return products.Cast<Product>();
            }

            public async Task<bool> UpdateProduct(Product product)
            {
                if (product is T typedProduct)
                {
                    return await _repository.UpdateProduct(typedProduct);
                }
                return false;
            }
        }

        [TestMethod]
        public void CanHandle_MatchingProductType_ReturnsTrue()
        {
            // Arrange
            var factory = new TestCustomProductRepositoryFactory(_serviceProvider);
            var product = new CustomProduct(
                "test", "Test Product", null, "", ProductCategory.Custom, "",
                null, new Dictionary<string, object>()
            );

            // Act & Assert
            Assert.IsTrue(factory.CanHandle(product));
        }

        [TestMethod]
        public void CanHandle_NonMatchingProductType_ReturnsFalse()
        {
            // Arrange
            var factory = new TestCustomProductRepositoryFactory(_serviceProvider);
            var product = new TourPackage();
            // Act & Assert
            Assert.IsFalse(factory.CanHandle(product));
        }

        [TestMethod]
        public void CanHandle_MatchingCategory_ReturnsTrue()
        {
            // Arrange
            var factory = new TestCustomProductRepositoryFactory(_serviceProvider);

            // Act & Assert
            Assert.IsTrue(factory.CanHandle(ProductCategory.Custom));
        }

        [TestMethod]
        public void CanHandle_NonMatchingCategory_ReturnsFalse()
        {
            // Arrange
            var factory = new TestCustomProductRepositoryFactory(_serviceProvider);

            // Act & Assert
            Assert.IsFalse(factory.CanHandle(ProductCategory.TourPackage));
        }

        [TestMethod]
        public void CreateRepository_ReturnsCorrectRepositoryType()
        {
            // Arrange
            var factory = new TestCustomProductRepositoryFactory(_serviceProvider);

            // Act
            var repository = factory.CreateRepository();

            // Assert
            Assert.IsNotNull(repository);
            Assert.IsInstanceOfType(repository, typeof(ProductRepositoryAdapter<CustomProduct>));
        }

        [TestMethod]
        public async Task CreateRepository_CanPerformRepositoryOperations()
        {
            // Arrange
            var factory = new TestCustomProductRepositoryFactory(_serviceProvider);
            var repository = factory.CreateRepository();

            var product = new CustomProduct(
                "test-repo-ops", "Repository Test Product", 
                Price.Create(99.99m, "USD"), 
                "Test description", 
                ProductCategory.Custom, 
                "TestProvider",
                new Core.Domain.Entities.SupportClasses.AvailabilityInfo("Available", 10),
                new Dictionary<string, object> { { "TestKey", "TestValue" } }
            );

            // Act - Add a product
            await repository.AddProductsAsync(new List<Product> { product });

            // Act - Get products
            var retrievedProducts = await repository.GetProductsAsync();

            // Assert
            Assert.IsNotNull(retrievedProducts);
            Assert.IsTrue(retrievedProducts.Any());
            var retrievedProduct = retrievedProducts.FirstOrDefault(p => p.ExternalId == "test-repo-ops");
            Assert.IsNotNull(retrievedProduct);
            Assert.AreEqual("Repository Test Product", retrievedProduct.Name);
            Assert.AreEqual(99.99m, retrievedProduct.Price.Amount);
        }

        [TestMethod]
        public void MultipleFactories_CorrectFactoryHandlesProduct()
        {
            // Arrange
            var customFactory = new TestCustomProductRepositoryFactory(_serviceProvider);
            
            // A mock factory for tour packages
            var tourFactory = new Mock<IProductRepositoryFactory>();
            tourFactory.Setup(f => f.CanHandle(It.IsAny<Product>()))
                .Returns((Product p) => p is TourPackage);
            tourFactory.Setup(f => f.CanHandle(ProductCategory.TourPackage))
                .Returns(true);

            var factories = new List<IProductRepositoryFactory> { customFactory, tourFactory.Object };

            // Create test products
            var customProduct = new CustomProduct(
                "custom1", "Custom Product", null, "", ProductCategory.Custom, "",
                null, new Dictionary<string, object>()
            );

            var tourProduct = new TourPackage();

            // Act & Assert
            var customFactory1 = factories.FirstOrDefault(f => f.CanHandle(customProduct));
            Assert.IsNotNull(customFactory1);
            Assert.AreEqual(customFactory, customFactory1);

            var tourFactory1 = factories.FirstOrDefault(f => f.CanHandle(tourProduct));
            Assert.IsNotNull(tourFactory1);
            Assert.AreEqual(tourFactory.Object, tourFactory1);
        }
    }
}