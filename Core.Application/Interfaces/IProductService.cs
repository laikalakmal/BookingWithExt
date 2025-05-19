using Core.Application.DTOs;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync();
}