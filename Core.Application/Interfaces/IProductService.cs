using Core.Application.DTOs;

namespace Core.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<int> SyncProductsFromExternalAsync();
        
    }
}