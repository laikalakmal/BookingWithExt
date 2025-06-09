using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IProductService
        
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(); /// Fetches all products from the repository and maps them to DTOs.
        Task<int> SyncProductsFromExternalAsync(); /// Syncs products from external sources and returns the total number of products synced.
        Task<IEnumerable<Product>> FetchExternalProductsAsync(); /// Fetches products from external sources and returns them as domain entities. this is used in the sync process.

        Task<ProductDto> GetByIdAsync(Guid id);

        ProductDto MapToDto(Product product);
        Product MapToDomain(ProductDto dto);
        Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto product, int quantity);

        Task<bool> DeleteProductAsync(Guid id);
    }
}