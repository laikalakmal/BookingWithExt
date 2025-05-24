using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;

namespace Infrastructure.Persistence.Factories
{
    public class ProductRepositoryFactory<TEntity> : IProductRepositoryFactory 
        where TEntity : Product
    {
        private readonly AppDbContext _context;
        private readonly ProductCategory _supportedCategory;
        private readonly Func<AppDbContext, IProductRepository<TEntity>> _repositoryFactory;

        public ProductRepositoryFactory(
            AppDbContext context, 
            ProductCategory supportedCategory,
            Func<AppDbContext, IProductRepository<TEntity>> repositoryFactory)
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
            return new ProductRepositoryAdapter<TEntity>(repository);
        }
    }
}