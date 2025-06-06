using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Mappings;
using Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Services.Concreate
{
    public class HolidayPackageService : IProductService<HolidayPackage, HolidayPackageDto>
    {
        private readonly IProductRepository<HolidayPackage> _repository;
        private readonly IExternalProductApiAdapter _adapter;

        public HolidayPackageService(
            [FromKeyedServices("holiday")] IProductRepository<HolidayPackage> repository,
            [FromKeyedServices("holiday")] IExternalProductApiAdapter externalProductApiAdapter)
        {
            _adapter = externalProductApiAdapter;
            _repository = repository;
        }

        public async Task<IEnumerable<HolidayPackageDto>> GetProductsAsync()
        {
            try
            {
                // Get local products from database
                var localProducts = await _repository.GetProductsAsync();
                // Map to dto
                return localProducts.OfType<HolidayPackage>().Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch products: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<HolidayPackage>> FetchExternalProductsAsync()
        {
            var allExternalProducts = new List<HolidayPackage>();

            try
            {
                // Fetch and map external products to domain models
                var products = await _adapter.FetchProductsAsync();
                allExternalProducts.AddRange(products.OfType<HolidayPackageDto>().Select(MapToDomain));
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
                    .Where(ext => !localProducts.OfType<HolidayPackage>().Any(local =>
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
                throw new Exception($"Failed to synchronize products from external sources: {ex.Message}", ex);
            }
        }

        public HolidayPackageDto MapToDto(HolidayPackage product)
        {
            try
            {
                return HolidayPackageMapper.FromDomain(product);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to map HolidayPackage to DTO: {ex.Message}", ex);
            }
        }

        public HolidayPackage MapToDomain(HolidayPackageDto dto)
        {
            try
            {
                return HolidayPackageMapper.ToDomain(dto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to map HolidayPackageDto to domain entity: {ex.Message}", ex);
            }
        }

        public async Task<HolidayPackageDto> GetByIdAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception($"HolidayPackage with ID {id} not found.");
            }
            return MapToDto(product);
        }

        public async Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto product, int quantity)
        {
            var response = await _adapter.PurchaseProductAsync(product, quantity);

            if (response.IsSuccess)
            {
                var holidayPackage = await _repository.GetByIdAsync(product.Id);
                if (holidayPackage != null)
                {
                    holidayPackage.Availability.RemainingSlots -= quantity;
                    await _repository.UpdateProduct(holidayPackage);
                }
            }

            return response;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var product= await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    throw new KeyNotFoundException($"HolidayPackage with ID {id} not found.");
                }
                return await _repository.DeleteProductAsync(product);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
