using Core.Application.DTOs;

namespace Core.Application.Interfaces
{

    // This interface is responsible for fetching products from an external API.
    // It abstracts the details of how products are retrieved, allowing for different implementations.
    //if you want to add a new adapter, you can implement this interface and register it in the DI container.

    public interface IExternalProductApiAdapter
    {


        string AdapterName { get; }

        Task<List<ProductDto>> FetchProductsAsync();

        Task<ProductDto?> FetchProductByIdAsync(string externalId);

        Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto product, int quantity);


    }
}


