using Core.Application.DTOs;
using Core.Domain.Entities;


namespace Core.Application.Interfaces
{
    public interface IAddableProduct<T,TDto>
        where T: Product
        where TDto:ProductDto
    {
        Task<Guid> AddProductAsync(TDto productRequest); 
    }
}
