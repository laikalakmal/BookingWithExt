using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class HolidayPackageRepository : IProductRepository<HolidayPackage>
    {
        private readonly AppDbContext _context;

        public HolidayPackageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProductsAsync(List<HolidayPackage> allProducts)
        {
            // Pattern matching with is-expression to collect HolidayPackage instances
            // This processes in one pass without creating multiple collections
            if (allProducts?.Any() != true)
                return;

            var entities = allProducts
                .Where(p => p is HolidayPackage)
                .Cast<HolidayPackage>();

            // Using AddRange is more efficient than multiple Add calls when possible
            if (entities.Any())
            {
                _context.HolidayPackages.AddRange(entities);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<HolidayPackage>> GetProductsAsync()
        {
            // Return holiday packages as products directly from the database
            return await _context.HolidayPackages.ToListAsync();
        }
    }
}
