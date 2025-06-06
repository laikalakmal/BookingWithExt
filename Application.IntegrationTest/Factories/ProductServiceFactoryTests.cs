using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Services.Concreate;
using Core.Application.Services.Factories;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Application.IntegrationTest.Factories
{
    [TestClass]
    public class ProductServiceFactoryTests
    {
        private IServiceProvider _serviceProvider;

        [TestInitialize]
        public void Setup()
        {
            // Create a service collection
            var services = new ServiceCollection();

            // Register mock repositories
            var mockCustomRepository = new Mock<IProductRepository<CustomProduct>>();
            var mockTourRepository = new Mock<IProductRepository<TourPackage>>();
            var mockHolidayRepository = new Mock<IProductRepository<HolidayPackage>>();

            // Register mock adapters
            var mockTourAdapter = new Mock<IExternalProductApiAdapter>();
            var mockHolidayAdapter = new Mock<IExternalProductApiAdapter>();

            // Register keyed services
            services.AddKeyedSingleton("CustomProduct", mockCustomRepository.Object);
            services.AddKeyedSingleton("TourPackage", mockTourRepository.Object);
            services.AddKeyedSingleton("HolidayPackage", mockHolidayRepository.Object);
            
            services.AddKeyedSingleton("TourPackage", mockTourAdapter.Object);
            services.AddKeyedSingleton("HolidayPackage", mockHolidayAdapter.Object);

            // Build the service provider
            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void CreateService_CustomProductCategory_ReturnsCustomProductService()
        {
            // Arrange
            var factory = new ProductServiceFactory<CustomProduct, CustomProductDto>(
                _serviceProvider,
                ProductCategory.Custom,
                "CustomProduct",
                (repo, adapter) => new CustomProductService(repo)
            );

            // Act
            var service = factory.CreateService();

            // Assert
            Assert.IsNotNull(service);
            // We need to check the inner service type, as it's wrapped in a ProductServiceAdapter
            var adaptedService = service as ProductServiceAdapter<CustomProduct, CustomProductDto>;
            Assert.IsNotNull(adaptedService, "Expected a ProductServiceAdapter");
            
            // Use reflection to access the private field
            var serviceField = typeof(ProductServiceAdapter<CustomProduct, CustomProductDto>)
                .GetField("_service", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var innerService = serviceField?.GetValue(adaptedService);
            
            Assert.IsNotNull(innerService);
            Assert.IsInstanceOfType(innerService, typeof(CustomProductService));
        }

        [TestMethod]
        public void CanHandle_MatchingCategory_ReturnsTrue()
        {
            // Arrange
            var factory = new ProductServiceFactory<CustomProduct, CustomProductDto>(
                _serviceProvider,
                ProductCategory.Custom,
                "CustomProduct",
                (repo, adapter) => new CustomProductService(repo)
            );

            // Act & Assert
            Assert.IsTrue(factory.CanHandle(ProductCategory.Custom));
        }

        [TestMethod]
        public void CanHandle_NonMatchingCategory_ReturnsFalse()
        {
            // Arrange
            var factory = new ProductServiceFactory<CustomProduct, CustomProductDto>(
                _serviceProvider,
                ProductCategory.Custom,
                "CustomProduct",
                (repo, adapter) => new CustomProductService(repo)
            );

            // Act & Assert
            Assert.IsFalse(factory.CanHandle(ProductCategory.HolidayPackage));
        }

        [TestMethod]
        public void CanHandle_MatchingProductCategory_ReturnsTrue()
        {
            // Arrange
            var factory = new ProductServiceFactory<CustomProduct, CustomProductDto>(
                _serviceProvider,
                ProductCategory.Custom,
                "CustomProduct",
                (repo, adapter) => new CustomProductService(repo)
            );

            var product = new CustomProduct(
                "test", "Test Product", null, "", ProductCategory.Custom, "",
                null, new Dictionary<string, object>()
            );

            // Act & Assert
            Assert.IsTrue(factory.CanHandle(product));
        }

        [TestMethod]
        public void CreateService_MissingRepository_ThrowsInvalidOperationException()
        {
            // Arrange - Create a factory with a non-existent key
            var factory = new ProductServiceFactory<CustomProduct, CustomProductDto>(
                _serviceProvider,
                ProductCategory.Custom,
                "NonExistentKey",
                (repo, adapter) => new CustomProductService(repo)
            );

            // Act & Assert
            Assert.ThrowsException<InvalidOperationException>(() => factory.CreateService());
        }

        [TestMethod]
        public void CreateService_WithAdapter_PassesAdapterToFactory()
        {
            // Arrange
            bool adapterPassed = false;
            
            var factory = new ProductServiceFactory<TourPackage, TourPackageDto>(
                _serviceProvider,
                ProductCategory.TourPackage,
                "TourPackage",
                (repo, adapter) => 
                {
                    // Check if the adapter was passed
                    adapterPassed = adapter != null;
                    return new Mock<IProductService<TourPackage, TourPackageDto>>().Object;
                }
            );

            // Act
            factory.CreateService();

            // Assert
            Assert.IsTrue(adapterPassed, "Adapter should be passed to the factory function");
        }
    }
}