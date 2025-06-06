using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Mappings;
using Core.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Services.Concreate
{
    public class CustomProductService : 
        IProductService<CustomProduct, CustomProductDto>,
        IEditableProduct<CustomProductDto>,
        IAddableProduct<CustomProduct,CustomProductDto>
    {
        private readonly IProductRepository<CustomProduct> _repository;

        public CustomProductService(
            [FromKeyedServices("custom")] IProductRepository<CustomProduct> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomProductDto>> GetProductsAsync()
        {
            try
            {
                var localProducts = await _repository.GetProductsAsync();
                if (localProducts == null)
                {
                    return [];
                }
                return localProducts.OfType<CustomProduct>().Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch custom products: {ex.Message}", ex);
            }
        }

        // Since there's no external API integration for custom products
        public Task<IEnumerable<CustomProduct>> FetchExternalProductsAsync()
        {
            // Return empty collection since we don't fetch from external sources
            return Task.FromResult<IEnumerable<CustomProduct>>(new List<CustomProduct>());
        }

        public Task<int> SyncProductsFromExternalAsync()
        {
            // No external syncing needed, return 0 as count of synced products
            return Task.FromResult(0);
        }

        public CustomProduct MapToDomain(CustomProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "CustomProductDto cannot be null.");

            return CustomProductMapper.ToDomain(dto);
        }

        public CustomProductDto MapToDto(CustomProduct product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "CustomProduct cannot be null.");

            return CustomProductMapper.FromDomain(product);
        }

        public async Task<CustomProductDto> GetByIdAsync(Guid id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id) ?? throw new Exception($"CustomProduct with ID {id} not found.");
                return MapToDto(product);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get custom product by ID: {ex.Message}", ex);
            }
        }

        public async Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto product, int quantity)
        {
            try
            {
                CustomProduct customProduct = await _repository.GetByIdAsync(product.Id) ?? throw new Exception($"Custom product with ID {product.Id} not found.");
                if (quantity <= 0 || quantity > customProduct.Availability.RemainingSlots)
                {
                    throw new Exception($"Invalid quantity {quantity} for product {product.Name}. Available slots: {customProduct.Availability.RemainingSlots}");
                }



                // Update inventory
                if (customProduct != null)
                {
                    customProduct.Availability.RemainingSlots -= quantity;
                    await _repository.UpdateProduct(customProduct);
                // Create a success response for internal custom products
                var response = new PurchaseResponseDto(product.ExternalId)
                {
                    TransactionId=Guid.NewGuid().ToString().Substring(8,0),
                    IsSuccess = true,
                    ProductId = product.Id,
                    Quantity = quantity,
                    TotalAmount = product.Price.Amount * quantity,
                    CurrencyCode = product.Price.Currency.ToString(),
                    PurchaseDate = DateTime.UtcNow,
                    Provider = customProduct?.Provider ?? "BookWithExt",
                    ConfirmationCode = Guid.NewGuid().ToString().Substring(0, 8),
                    Message = "Purchase successful"
                };

                return response;

                }
                else
                {
                    throw new Exception($"Custom product with ID {product.Id} not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to process purchase: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"CustomProduct with ID {id} not found.");
            }
            try
            {
                return await _repository.DeleteProductAsync(product);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete custom product: {ex.Message}", ex);
            }
        }


        public async Task<bool> EditProduct(Guid productId, CustomProductDto updatedProductDto)
        {
            ArgumentNullException.ThrowIfNull(updatedProductDto);

            try
            {
                
                var existingProduct = await _repository.GetByIdAsync(productId) ?? 
                    throw new KeyNotFoundException($"CustomProduct with ID {productId} not found.");
                    
                
                var newProduct = MapToDomain(updatedProductDto);
                
                existingProduct.Name = newProduct.Name;
                existingProduct.Price = newProduct.Price;
                existingProduct.Description = newProduct.Description;
                existingProduct.Category = newProduct.Category;
                existingProduct.Provider = newProduct.Provider;
                existingProduct.Availability = newProduct.Availability;
                existingProduct.Attributes = newProduct.Attributes;
                existingProduct.ImageUrl = newProduct.ImageUrl ?? string.Empty;
                existingProduct.UpdatedAt = DateTime.UtcNow;
                
               
                
                // Save changes
                return await _repository.UpdateProduct(existingProduct);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to edit custom product: {ex.Message}", ex);
            }
        }

        public async Task<Guid> AddProductAsync(CustomProductDto productRequest)
        {
            ArgumentNullException.ThrowIfNull(productRequest);

            try
            {

                var product = MapToDomain(productRequest);
                
                
                product.Id = Guid.NewGuid();
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;
                
                await _repository.AddProductsAsync(new List<CustomProduct> { product });
                
                return product.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add custom product: {ex.Message}", ex);
            }
        }
    }
}
