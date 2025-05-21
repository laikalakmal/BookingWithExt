using Core.Application.DTOs;

namespace Core.Application.Interfaces
{
    public interface IExternalProductApiAdapter
    {
        Task<List<ProductDto>> FetchProductsAsync();


    }
}
