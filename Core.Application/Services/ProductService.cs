using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Mappings;
using Core.Domain.Entities;

namespace Core.Application.Services
{
    public class ProductService(
        IProductRepository repository,
        IEnumerable<IExternalProductApiAdapter> externalApiAdapters) : IProductService
    {
        private readonly IProductRepository _repository = repository;
        private readonly IEnumerable<IExternalProductApiAdapter> _externalApiAdapters = externalApiAdapters;

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            // Get local products from database
            var localProducts = await _repository.GetProductsAsync();

            // Fetch products from all external APIs
            var externalProducts = await FetchExternalProductsAsync();

            // Merge and deduplicate
            var allProducts = MergeProducts(localProducts, externalProducts);

            // Identify products to add or update in the database
            var productsToSync = externalProducts
                .Where(ext => !localProducts.Any(local =>
                    local.ExternalId == ext.ExternalId && local.UpdatedAt >= ext.UpdatedAt))
                .ToList();

            // Persist new or updated external products to database
            if (productsToSync.Any())
            {
                await _repository.AddProductsAsync(productsToSync);
            }

            // Map to DTOs
            return allProducts.Select(MapToDto);
        }

        private async Task<IEnumerable<Product>> FetchExternalProductsAsync()
        {
            var allExternalProducts = new List<Product>();

            foreach (var adapter in _externalApiAdapters)
            {
                try
                {
                    // Fetch and map external products to domain models
                    var products = await adapter.FetchProductsAsync();
                    allExternalProducts.AddRange(products.Select(MapToDomain));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch from {adapter.GetType().Name}: {ex.Message}");
                }
            }

            return allExternalProducts;
        }

        private IEnumerable<Product> MergeProducts(
            IEnumerable<Product> localProducts,
            IEnumerable<Product> externalProducts)
        {
            // Deduplication by ExternalId (last updated wins)
            return localProducts
                .Concat(externalProducts)
                .GroupBy(p => p.ExternalId)
                .Select(g => g.OrderByDescending(p => p.UpdatedAt).First());
        }

        private Product MapToDomain(ProductDto dto)
        {
            return dto switch
            {
                TourPackageDto tourDto => TourPackageMapper.ToDomain(tourDto),
                _ => throw new NotSupportedException($"Unsupported DTO type: {dto.GetType().Name}")
            };
        }

        private ProductDto MapToDto(Product product)
        {
            return product switch
            {
                TourPackage tour => TourPackageMapper.FromDomain(product),
                _ => throw new NotSupportedException($"Unsupported product type: {product.GetType().Name}")
            };
        }

        public async Task<int> SyncProductsFromExternalAsync()
        {
            // Get local products from database
            var localProducts = await _repository.GetProductsAsync();

            // Fetch products from all external APIs
            var externalProducts = await FetchExternalProductsAsync();

            // Identify products to add or update in the database
            var productsToSync = externalProducts
                .Where(ext => !localProducts.Any(local =>
                    local.ExternalId == ext.ExternalId && local.UpdatedAt >= ext.UpdatedAt))
                .ToList();

            // Persist new or updated external products to database
            if (productsToSync.Any())
            {
                await _repository.AddProductsAsync(productsToSync);
            }

            // Return count of synced products
            return productsToSync.Count;
        }
    }
}