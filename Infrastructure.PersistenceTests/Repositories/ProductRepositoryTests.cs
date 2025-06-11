using Core.Domain.Entities;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Infrastructure.Persistence.Repositories.Tests
{
    [TestClass()]
    public class ProductRepositoryTests
    {
        private AppDbContext _context;
        private List<Product> _products;
        private ProductRepository _repository;

        [TestInitialize]
        public void Initialize()
        {
            // Create an in-memory database instead of mocking the context
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductsTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);

            // Create sample products for testing with all required properties
            _products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    ExternalId = "ext-1",
                    Name = "Product 1",
                    Description = "Description 1",
                    Price = Price.Create(10.99m, "USD"),
                    Category = ProductCategory.Custom,
                    Provider = "Provider1",
                    Availability = new AvailabilityInfo("Available", 10),
                    Attributes = new Dictionary<string, object>(),
                    ImageUrl = new List<string> { "https://example.com/image1.jpg" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    ExternalId = "ext-2",
                    Name = "Product 2",
                    Description = "Description 2",
                    Price = Price.Create(20.99m, "USD"),
                    Category = ProductCategory.Custom,
                    Provider = "Provider2",
                    Availability = new AvailabilityInfo("Available", 5),
                    Attributes = new Dictionary<string, object>(),
                    ImageUrl = new List<string> { "https://example.com/image2.jpg" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            // Add products to the in-memory database
            _context.Products.AddRange(_products);
            _context.SaveChanges();

            // Create repository with real context using in-memory database
            _repository = new ProductRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod()]
        public void ProductRepositoryTest()
        {
            // Arrange & Act - using in-memory db context
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestConstructorDb")
                .Options;
            var context = new AppDbContext(options);
            var repository = new ProductRepository(context);

            // Assert
            Assert.IsNotNull(repository);
            
            // Cleanup
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [TestMethod()]
        public async Task AddProductsAsyncTest()
        {
            // Arrange
            var initialCount = _products.Count;
            var newProducts = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    ExternalId = "ext-3",
                    Name = "Product 3",
                    Description = "Description 3",
                    Price = Price.Create(30.99m, "USD"),
                    Category = ProductCategory.Custom,
                    Provider = "Provider3",
                    Availability = new AvailabilityInfo("Available", 15),
                    Attributes = new Dictionary<string, object>(),
                    ImageUrl = new List<string> { "https://example.com/image3.jpg" }
                }
            };

            // Act
            await _repository.AddProductsAsync(newProducts);

            // Assert
            var allProducts = await _context.Products.ToListAsync();
            Assert.AreEqual(initialCount + 1, allProducts.Count);
            Assert.IsTrue(allProducts.Any(p => p.Name == "Product 3"));
        }

        [TestMethod()]
        public async Task GetByIdAsyncTest()
        {
            // Arrange
            var expectedProduct = _products.First();

            // Act
            var result = await _repository.GetByIdAsync(expectedProduct.Id);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedProduct.Id, result.Id);
            Assert.AreEqual(expectedProduct.Name, result.Name);
            Assert.AreEqual(expectedProduct.Price.Amount, result.Price.Amount);
            Assert.AreEqual(expectedProduct.Category, result.Category);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task GetByIdAsyncTest_NotFound_ThrowsException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            await _repository.GetByIdAsync(nonExistentId);

            // Assert - Exception expected
        }

        [TestMethod()]
        public async Task GetProductsAsyncTest()
        {
            // Act
            var result = await _repository.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_products.Count, result.Count());

            // Verify all products are returned
            foreach (var product in _products)
            {
                Assert.IsTrue(result.Any(p => p.Id == product.Id));
            }
        }

        [TestMethod()]
        public async Task UpdateProductTest()
        {
            // Arrange
            var productToUpdate = _products.First();
            var updatedProduct = new Product
            {
                Id = productToUpdate.Id,
                ExternalId = productToUpdate.ExternalId,
                Name = "Updated Name",
                Description = "Updated Description",
                Price = Price.Create(15.99m, "USD"),
                Category = ProductCategory.TourPackage, // Changed category
                Provider = productToUpdate.Provider,
                Availability = new AvailabilityInfo("Limited", 3), // Changed availability
                Attributes = new Dictionary<string, object>
                {
                    { "feature", "premium" }
                },
                ImageUrl = new List<string> { "https://example.com/updated.jpg" },
                CreatedAt = productToUpdate.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.UpdateProduct(updatedProduct);
            

            // Assert
            Assert.IsTrue(result);
            
            // Verify product was actually updated in the database
            var updatedInDb = await _context.Products.FindAsync(productToUpdate.Id);
            Assert.IsNotNull(updatedInDb);
            Assert.AreEqual("Updated Name", updatedInDb.Name);
            Assert.AreEqual("Updated Description", updatedInDb.Description);
            Assert.AreEqual(ProductCategory.TourPackage, updatedInDb.Category);
            Assert.AreEqual("Limited", updatedInDb.Availability.Status);
            Assert.AreEqual(3, updatedInDb.Availability.RemainingSlots);
        }

        [TestMethod()]
        public async Task UpdateProductTest_NotFound_ReturnsFalse()
        {
            // Arrange
            var nonExistentProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Non-existent Product",
                Price = Price.Create(10.99m, "USD"),
                Category = ProductCategory.Custom,
                Provider = "Provider",
                Availability = new AvailabilityInfo("Available", 10),
                Attributes = new Dictionary<string, object>(),
                ImageUrl = new List<string>()
            };

            // Act
            var result = await _repository.UpdateProduct(nonExistentProduct);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task DeleteProductAsyncTest_ById()
        {
            // Arrange
            var productToDelete = _products.First();
            var initialCount = _products.Count;

            // Act
            var result = await _repository.DeleteProductAsync(productToDelete.Id);

            // Assert
            Assert.IsTrue(result);
            var allProducts = await _context.Products.ToListAsync();
            Assert.AreEqual(initialCount - 1, allProducts.Count);
            Assert.IsFalse(allProducts.Any(p => p.Id == productToDelete.Id));
        }

        [TestMethod()]
        public async Task DeleteProductAsyncTest_ById_NotFound_ReturnsFalse()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.DeleteProductAsync(nonExistentId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task DeleteProductAsyncTest_ByObject()
        {
            // Arrange
            var productToDelete = _products.First();
            var initialCount = _products.Count;

            // Act
            var result = await _repository.DeleteProductAsync(productToDelete);

            // Assert
            Assert.IsTrue(result);
            var allProducts = await _context.Products.ToListAsync();
            Assert.AreEqual(initialCount - 1, allProducts.Count);
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task DeleteProductAsyncTest_ByObject_Null_ThrowsException()
        {
            // Act
            await _repository.DeleteProductAsync(null);

            // Assert - Exception expected
        }
    }
}