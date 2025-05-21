using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IProductService<TDomain, TDto>
        where TDto : ProductDto
        where TDomain : Product
    {
        Task<IEnumerable<TDto>> GetProductsAsync();
        Task<int> SyncProductsFromExternalAsync();
        Task<IEnumerable<TDomain>> FetchExternalProductsAsync();

        TDto MapToDto(TDomain product);
        TDomain MapToDomain(TDto dto);
    }
}