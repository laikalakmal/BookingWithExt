using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;

namespace Core.Application.Services.Factories
{
    public class ProductServiceAdapter<TEntity, TDto> : IProductService<Product, ProductDto>
        where TEntity : Product
        where TDto : ProductDto
    {
        private readonly IProductService<TEntity, TDto> _service;

        public ProductServiceAdapter(IProductService<TEntity, TDto> service)
        {
            _service = service;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _service.GetProductsAsync();
            return products.Cast<ProductDto>();
        }

        public async Task<IEnumerable<Product>> FetchExternalProductsAsync()
        {
            var products = await _service.FetchExternalProductsAsync();
            return products.Cast<Product>();
        }

        public async Task<int> SyncProductsFromExternalAsync()
        {
            return await _service.SyncProductsFromExternalAsync();
        }

        public ProductDto MapToDto(Product product)
        {
            if (product is TEntity typedProduct)
            {
                return _service.MapToDto(typedProduct);
            }

            throw new InvalidOperationException(
                $"Cannot map product of type {product.GetType().Name} to {typeof(TDto).Name}");
        }

        public Product MapToDomain(ProductDto dto)
        {
            if (dto is TDto typedDto)
            {
                return _service.MapToDomain(typedDto);
            }

            throw new InvalidOperationException(
                $"Cannot map DTO of type {dto.GetType().Name} to {typeof(TEntity).Name}");
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto;
        }

        public Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto product, int quantity)
        {
            if (product is TDto typedDto)
            {
                // Delegate to the underlying service, casting as needed
                return _service.PurchaseProductAsync(typedDto, quantity);
            }

            throw new InvalidOperationException(
                $"Cannot purchase product of type {product.GetType().Name} with adapter for {typeof(TDto).Name}");
        }

        public Task<bool> DeleteProductAsync(Guid id)
        {
            return _service.DeleteProductAsync(id);
        }
    }
}