using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Enums;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Concreate;

namespace Infrastructure.Persistence.Factories
{
    public class TourPackageRepositoryFactory : IProductRepositoryFactory
    {
        private readonly AppDbContext _context;

        public TourPackageRepositoryFactory(AppDbContext context)
        {
            _context = context;
        }

        public bool CanHandle(Product product)
        {
            return product.Category == ProductCategory.TourPackage;
        }

        public IProductRepository<Product> CreateRepository()
        {
            return (IProductRepository<Product>)(object)new TourPackageRepository(_context);
        }
    }
}