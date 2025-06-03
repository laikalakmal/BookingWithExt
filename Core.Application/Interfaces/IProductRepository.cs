using Core.Domain.Entities;

namespace Core.Application.Interfaces
{

    // implement this for each product type.
    // this is a generic interface for the product repository.
    // it is used to add and get products from the database.
    // it is used in the product service to add and get products from the database.
    // each implimentation should have a factory that creates the product repository.
    // the factory should be registered in the DI container as well as the repository itself.
    public interface IProductRepository<T> where T : Product
    {
        Task AddProductsAsync(List<T> products);
        Task<IEnumerable<T>> GetProductsAsync();

        Task<T> GetByIdAsync(Guid id);
        Task<bool> UpdateProduct(T product);

        Task<bool> DeleteProductAsync(Guid id);
    }
}
