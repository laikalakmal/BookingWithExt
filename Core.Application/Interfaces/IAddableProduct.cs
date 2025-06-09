using Core.Application.DTOs;
using Core.Domain.Entities;


namespace Core.Application.Interfaces
{
    public interface IAddableProduct
    {
        Task<Guid> AddProductAsync(ProductDto productRequest);
    }
}
