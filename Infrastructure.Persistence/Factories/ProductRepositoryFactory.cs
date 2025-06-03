using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Infrastructure.Persistence.Factories
{
    public class ProductRepositoryFactory<T> : IProductRepositoryFactory
        where T : Product
    {
        private readonly AppDbContext _context;
        private readonly ProductCategory _supportedCategory;
        private readonly Func<AppDbContext, IProductRepository<T>> _repositoryFactory;

        public ProductRepositoryFactory(
            AppDbContext context,
            ProductCategory supportedCategory,
            Func<AppDbContext, IProductRepository<T>> repositoryFactory)
        {
            _context = context;
            _supportedCategory = supportedCategory;
            _repositoryFactory = repositoryFactory;
        }

        public bool CanHandle(Product product)
        {
            return product.Category == _supportedCategory;
        }

        public IProductRepository<Product> CreateRepository()
        {
            var repository = _repositoryFactory(_context);
            return new ProductRepositoryAdapter<T>(repository);
        }
    }
}