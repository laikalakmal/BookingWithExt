using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Infrastructure.Persistence.Repositories
{
    // this class is responsible for adding products to the database without reliance on a specific repository.
    // It uses the repository factories to determine which repository to use for each product based on its category.
    // so you don't need to worry about which repository to use when adding products.
    // The ProductRepository class will handle that for you.

    public class GenericProductRepository : IProductRepository<Product>
    {
        private readonly IEnumerable<IProductRepositoryFactory> _repositoryFactories;
        private readonly Dictionary<Guid, ProductCategory> _productCategoryCache = new();

        public GenericProductRepository(IEnumerable<IProductRepositoryFactory> repositoryFactories)
        {
            _repositoryFactories = repositoryFactories ?? throw new ArgumentNullException(nameof(repositoryFactories));
        }

        // Add a method to get the appropriate repository factory for a category
        private IProductRepositoryFactory GetFactoryForCategory(ProductCategory category)
        {
            var factory = _repositoryFactories.FirstOrDefault(f => f.CanHandle(category));
            if (factory == null)
                throw new InvalidOperationException($"No repository found for product category {category}.");
            return factory;
        }

        public async Task AddProductsAsync(List<Product> products)
        {
            Dictionary<IProductRepository<Product>, List<Product>> productsByRepo = GroupProductsByRepository(products);

            foreach (var (repo, productGroup) in productsByRepo)
            {
                await repo.AddProductsAsync(productGroup);

                // Update our cache with the new products
                foreach (var product in productGroup)
                {
                    _productCategoryCache[product.Id] = product.Category;
                }
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                // First try to use our cache to find the category
                if (_productCategoryCache.TryGetValue(id, out var category))
                {
                    var factory = GetFactoryForCategory(category);
                    var repo = factory.CreateRepository();
                    var deleted = await repo.DeleteProductAsync(id);
                    if (deleted)
                    {
                        _productCategoryCache.Remove(id);
                        return true;
                    }
                }

                // Fall back to the original approach if not in cache
                foreach (var factory in _repositoryFactories)
                {
                    try
                    {
                        var repo = factory.CreateRepository();
                        var deleted = await repo.DeleteProductAsync(id);
                        if (deleted)
                        {
                            _productCategoryCache.Remove(id);
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                        // Continue to next repository if this one fails
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting product with ID {id}", ex);
            }
        }


        public async Task<bool> DeleteProductAsync(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);
            var factory = _repositoryFactories.FirstOrDefault(f => f.CanHandle(product)) ?? throw new InvalidOperationException($"No repository found for product category {product.Category}.");
            var repo = factory.CreateRepository();
            var result = await repo.DeleteProductAsync(product);
            if (result)
            {
                // Update our cache
                _productCategoryCache.Remove(product.Id);
            }
            return result;
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            try
            {
                // First try to use our cache to find the category
                if (_productCategoryCache.TryGetValue(id, out var category))
                {
                    var factory = GetFactoryForCategory(category);
                    var repo = factory.CreateRepository();
                    var product = await repo.GetByIdAsync(id);
                    if (product != null)
                    {
                        return product;
                    }
                }

                // Fall back to the original approach if not in cache
                foreach (var factory in _repositoryFactories)
                {
                    try
                    {
                        var repo = factory.CreateRepository();
                        var product = await repo.GetByIdAsync(id);
                        if (product != null)
                        {
                            // Update cache for future lookups
                            _productCategoryCache[id] = product.Category;
                            return product;
                        }
                    }
                    catch (Exception)
                    {
                        // Continue to the next factory if this one fails
                    }
                }

                throw new KeyNotFoundException($"No product found with ID {id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product with ID {id}: " + ex.Message, ex);
            }
        }

        // Update the cache when getting all products
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            try
            {
                var results = new List<Product>();

                foreach (var factory in _repositoryFactories)
                {
                    var repo = factory.CreateRepository();
                    var products = await repo.GetProductsAsync();

                    // Update our cache with all products
                    foreach (var product in products)
                    {
                        _productCategoryCache[product.Id] = product.Category;
                    }

                    results.AddRange(products);
                }

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Update the cache when updating a product
        public async Task<bool> UpdateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var factory = _repositoryFactories.FirstOrDefault(f =>
                f.CanHandle(product));

            if (factory == null)
                throw new InvalidOperationException($"No repository found for product category {product.Category}.");

            var repo = factory.CreateRepository();
            var result = await repo.UpdateProduct(product);

            if (result)
            {
                // Update our cache
                _productCategoryCache[product.Id] = product.Category;
            }

            return result;
        }

        private Dictionary<IProductRepository<Product>, List<Product>> GroupProductsByRepository(List<Product> products)
        {
            try
            {
                var result = new Dictionary<IProductRepository<Product>, List<Product>>();

                foreach (var product in products)
                {
                    var factory = _repositoryFactories.FirstOrDefault(f => f.CanHandle(product));

                    if (factory == null)
                        throw new InvalidOperationException($"No repository found for product category {product.Category}.");

                    var repo = factory.CreateRepository();

                    if (!result.ContainsKey(repo))
                    {
                        result[repo] = new List<Product>();
                    }

                    result[repo].Add(product);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(" Error at Repository factory", ex);
            }
        }
    }
}
