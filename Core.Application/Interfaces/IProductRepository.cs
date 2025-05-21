using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IProductRepository<T> where T : Product
    {
        Task AddProductsAsync(List<T> products);
        Task<IEnumerable<T>> GetProductsAsync();
    }
}
