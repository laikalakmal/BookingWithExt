using Core.Application.Interfaces;
using Core.Domain.Entities;

namespace Infrastructure.Persistence.Factories
{
    public class ProductRepositoryAdapter<T> : IProductRepository<Product> where T : Product
    {
        private readonly IProductRepository<T> _repository;

        public ProductRepositoryAdapter(IProductRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task AddProductsAsync(List<Product> products)
        {
            var typedProducts = products.OfType<T>().ToList();
            await _repository.AddProductsAsync(typedProducts);
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var typedProduct = await _repository.GetByIdAsync(id);
            return typedProduct as Product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = await _repository.GetProductsAsync();
            return products.Cast<Product>();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            if (product is not T typedProduct)
                return false;
            return await _repository.UpdateProduct(typedProduct);
        }
    }
}