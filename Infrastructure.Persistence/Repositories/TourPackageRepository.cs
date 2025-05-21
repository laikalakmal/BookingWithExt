using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TourPackageRepository : IProductRepository<TourPackage>
    {
        private readonly AppDbContext _context;

        public TourPackageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProductsAsync(List<TourPackage> allProducts)
        {

            if (allProducts?.Any() != true)
                return;

            var entities = allProducts
                .Where(p => p is TourPackage)
                .Cast<TourPackage>();


            if (entities.Any())
            {
                _context.TourPackages.AddRange(entities);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TourPackage>> GetProductsAsync()
        {

            return await _context.TourPackages.ToListAsync();
        }
    }
}
