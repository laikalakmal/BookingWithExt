using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IExternalProductApiAdapter
    {
        Task<List<ProductDto>> FetchProductsAsync();


    }
}
