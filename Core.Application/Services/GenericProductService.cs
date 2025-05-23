using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Core.Application.Services
{
    // this is a generic product service that uses factories to create specific product services based on the product type.
    // It allows you to handle different product types without needing to know the specifics of each type.
    // The service will delegate the actual work to the appropriate product service based on the product's category.
    public class GenericProductService : IProductService<Product, ProductDto>
    {
        private readonly IEnumerable<IProductServiceFactory> _serviceFactories;
        private readonly IProductRepository<Product> _productRepository;

        public GenericProductService(
            IEnumerable<IProductServiceFactory> serviceFactories,
            IProductRepository<Product> productRepository)
        {
            _serviceFactories = serviceFactories ?? throw new ArgumentNullException(nameof(serviceFactories));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            IEnumerable<Product> products = await _productRepository.GetProductsAsync();
            List<ProductDto> results = new List<ProductDto>();
            
            foreach (var product in products)
            {
                IProductServiceFactory factory = GetFactoryForProduct(product);
                IProductService<Product, ProductDto> service = factory.CreateService();
                results.Add(service.MapToDto(product));
            }
            
            return results;
        }

        public async Task<int> SyncProductsFromExternalAsync()
        {
            int totalSynced = 0;
            
            foreach (IProductServiceFactory factory in _serviceFactories)
            {
                IProductService<Product, ProductDto> service = factory.CreateService();
                totalSynced += await service.SyncProductsFromExternalAsync();
            }
            
            return totalSynced;
        }

        public async Task<IEnumerable<Product>> FetchExternalProductsAsync()
        {
            var results = new List<Product>();
            
            foreach (var factory in _serviceFactories)
            {
                var service = factory.CreateService();
                var products = await service.FetchExternalProductsAsync();
                results.AddRange(products);
            }
            
            return results;
        }

        public ProductDto MapToDto(Product product)
        {
            var factory = GetFactoryForProduct(product);
            var service = factory.CreateService();
            return service.MapToDto(product);
        }

        public Product MapToDomain(ProductDto dto)
        {
            // Instead of creating a Product instance, find factory based on DTO category
            var factory = FindFactoryByCategory(dto.Category);
            
            if (factory == null)
                throw new InvalidOperationException($"No service found for product category {dto.Category}.");
                
            var service = factory.CreateService();
            return service.MapToDomain(dto);
        }

        private IProductServiceFactory GetFactoryForProduct(Product product)
        {
            IProductServiceFactory? factory = _serviceFactories.FirstOrDefault(f => f.CanHandle(product));
            
            if (factory == null)
                throw new InvalidOperationException($"No service found for product category {product.Category}.");
                
            return factory;
        }
        
        private IProductServiceFactory? FindFactoryByCategory(ProductCategory category)
        {
            // Try to find a factory that can handle this category
            // We'll rely on each factory implementation to check just the category
            return _serviceFactories.FirstOrDefault(f => 
                f.GetType().Name.StartsWith(category.ToString()) || 
                // This is a fallback check to see if any factory advertises itself as handling this category
                (f is IProductServiceFactory factory && 
                 factory.CanHandle(GetSampleProductForCategory(category))));
        }

        private Product GetSampleProductForCategory(ProductCategory category)
        {
            // Return a concrete implementation based on category to test with CanHandle
            switch (category)
            {
                case ProductCategory.TourPackage:
                    return new TourPackage();
                    
                case ProductCategory.HolidayPackage:
                    return new HolidayPackage();
                    
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), 
                        $"No concrete product implementation exists for category {category}");
            }
        }
    }
}