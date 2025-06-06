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

        public Task<bool> DeleteProductAsync(Guid id)
        {
           if (id == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty.", nameof(id));
            return _repository.DeleteProductAsync(id);
        }

        public Task<bool> DeleteProductAsync(Product product)
        {
            if(product == null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            if (product is not T typedProduct)
                throw new InvalidCastException($"Cannot cast {product.GetType().Name} to {typeof(T).Name}.");
            return _repository.DeleteProductAsync(typedProduct);
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