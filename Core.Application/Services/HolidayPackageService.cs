using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Mappings;
using Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Services
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
            // Get local products from database
            var localProducts = await _repository.GetProductsAsync();
            // Map to dto
            return localProducts.OfType<HolidayPackage>().Select(MapToDto).ToList();
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



        public HolidayPackageDto MapToDto(HolidayPackage product)
        {
            return HolidayPackageMapper.FromDomain(product);
        }

        public HolidayPackage MapToDomain(HolidayPackageDto dto)
        {
            return HolidayPackageMapper.ToDomain(dto);
        }
    }
}
