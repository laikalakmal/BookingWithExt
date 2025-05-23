using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Concreate;

namespace Infrastructure.Persistence.Factories
{
    public class HolidayPackageRepositoryFactory : IProductRepositoryFactory
    {
        private readonly AppDbContext _context;

        public HolidayPackageRepositoryFactory(AppDbContext context)
        {
            _context = context;
        }

        public bool CanHandle(Product product)
        {
            return product.Category == ProductCategory.HolidayPackage;
        }

        public IProductRepository<Product> CreateRepository()
        {
            // Cast needed because of covariance constraints
            return (IProductRepository<Product>)(object)new HolidayPackageRepository(_context);
        }
    }
}