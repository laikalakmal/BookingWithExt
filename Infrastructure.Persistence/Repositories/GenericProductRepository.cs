using Core.Application.Interfaces;
using Core.Domain.Entities;

namespace Infrastructure.Persistence.Repositories
{
    // this class is responsible for adding products to the database without reliance on a specific repository.
    // It uses the repository factories to determine which repository to use for each product based on its category.
    // so you don't need to worry about which repository to use when adding products.
    // The ProductRepository class will handle that for you.

    public class GenericProductRepository : IProductRepository<Product>
    {
        private readonly IEnumerable<IProductRepositoryFactory> _repositoryFactories;

        public GenericProductRepository(IEnumerable<IProductRepositoryFactory> repositoryFactories)
        {
            _repositoryFactories = repositoryFactories ?? throw new ArgumentNullException(nameof(repositoryFactories));
        }

        public async Task AddProductsAsync(List<Product> products)
        {
            Dictionary<IProductRepository<Product>, List<Product>> productsByRepo = GroupProductsByRepository(products);

            foreach (var (repo, productGroup) in productsByRepo)
            {
                await repo.AddProductsAsync(productGroup);
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            foreach (var factory in _repositoryFactories)
            {
                try
                {
                    var repo = factory.CreateRepository();
                    var deleted = await repo.DeleteProductAsync(id);
                    if (deleted)
                    {
                        Console.WriteLine("deleted\n "+repo?.ToString()+"\n");
                        return true;
                    }
                }
                catch (Exception)
                {
                    // Continue to next repository if this one fails
                }
            }

            return false; // No repository could delete the product
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            try
            {
                foreach (var factory in _repositoryFactories)
                {
                    try
                    {
                        var repo = factory.CreateRepository();
                        var product = await repo.GetByIdAsync(id);
                        if (product != null)
                        {
                            return product;
                        }
                    }
                    catch (Exception)
                    {

                        // ctninue to the next factory if this one fails
                    }
                }

                throw new KeyNotFoundException($"No product found with ID {id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product with ID {id}: "+ex.Message, ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            try
            {
                var results = new List<Product>();

                foreach (var factory in _repositoryFactories)
                {
                    var repo = factory.CreateRepository();
                    var products = await repo.GetProductsAsync();
                    results.AddRange(products);
                }

                return results;
            }
            catch (Exception)
            {
                throw;

            }
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var factory = _repositoryFactories.FirstOrDefault(f =>
                f.CanHandle(product));

            if (factory == null)
                throw new InvalidOperationException($"No repository found for product category {product.Category}.");

            var repo = factory.CreateRepository();

            return await repo.UpdateProduct(product);
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
