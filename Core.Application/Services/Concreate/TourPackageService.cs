using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Mappings;
using Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Services.Concreate
{
    public class TourPackageService : IProductService<TourPackage, TourPackageDto>
    {
        private readonly IProductRepository<TourPackage> _repository;
        private readonly IExternalProductApiAdapter _adapter;

        public TourPackageService(
            [FromKeyedServices("tour")] IProductRepository<TourPackage> repository,
            [FromKeyedServices("tour")] IExternalProductApiAdapter externalProductApiAdapter)
        {
            _adapter = externalProductApiAdapter;
            _repository = repository;
        }

        public async Task<IEnumerable<TourPackageDto>> GetProductsAsync()
        {
            try
            {
                // Get local products from database
                var localProducts = await _repository.GetProductsAsync();
                // Map to dto
                return localProducts.OfType<TourPackage>().Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tour packages: {ex.Message}");
                return Enumerable.Empty<TourPackageDto>();
            }
        }

        public async Task<IEnumerable<TourPackage>> FetchExternalProductsAsync()
        {
            var allExternalProducts = new List<TourPackage>();

            try
            {
                // Fetch and map external products to domain models
                var products = await _adapter.FetchProductsAsync();
                allExternalProducts.AddRange(products.OfType<TourPackageDto>().Select(MapToDomain));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch from {_adapter.GetType().Name}: {ex.Message}");
            }

            return allExternalProducts;
        }

        public async Task<int> SyncProductsFromExternalAsync()
        {
            try
            {
                var localProducts = await _repository.GetProductsAsync();
                var externalProducts = await FetchExternalProductsAsync();

                // Identify products to add or update in the database
                var productsToSync = externalProducts
                    .Where(ext => !localProducts.OfType<TourPackage>().Any(local =>
                        local.ExternalId == ext.ExternalId))
                    .ToList();
                //to-do : add update logic

                // Persist new or updated external products to database
                if (productsToSync.Any())
                {
                    await _repository.AddProductsAsync(productsToSync);
                }

                // Return count of synced products
                return productsToSync.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error syncing products from external source: {ex.Message}");
                return 0;
            }
        }

        public TourPackageDto MapToDto(TourPackage product)
        {
            try
            {
                return TourPackageMapper.FromDomain(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mapping tour package to DTO: {ex.Message}");
                throw;
            }
        }

        public TourPackage MapToDomain(TourPackageDto dto)
        {
            try
            {
                return TourPackageMapper.ToDomain(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error mapping DTO to tour package: {ex.Message}");
                throw;
            }
        }

        public async Task<TourPackageDto> GetByIdAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Tour package with ID {id} not found.");
            }
            return MapToDto(product);
        }

        public async Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto product, int quantity)
        {
            var tourPackage = await _repository.GetByIdAsync(product.Id);
            var response = await _adapter.PurchaseProductAsync(product, quantity);

            if (response.IsSuccess)
            {
                if (tourPackage != null)
                {
                    tourPackage.Availability.RemainingSlots -= quantity;
                    bool updateSucceeded = await _repository.UpdateProduct(tourPackage);

                    if (!updateSucceeded)
                    {
                        throw new Exception($"Failed to update tour package with ID {tourPackage.Id} after purchase.");
                    }
                }
            }

            return response;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Tour package with ID {id} not found.");
                }
                return await _repository.DeleteProductAsync(id);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
