using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IProductService<TDomain, TDto>
        where TDto : ProductDto
        where TDomain : Product
    {
        Task<IEnumerable<TDto>> GetProductsAsync(); /// Fetches all products from the repository and maps them to DTOs.
        Task<int> SyncProductsFromExternalAsync(); /// Syncs products from external sources and returns the total number of products synced.
        Task<IEnumerable<TDomain>> FetchExternalProductsAsync(); /// Fetches products from external sources and returns them as domain entities. this is used in the sync process.

        TDto MapToDto(TDomain product);
        TDomain MapToDomain(TDto dto);
    }
}