using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Concreate
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
            try
            {
                if (allProducts?.Any() != true)
                    return;

                var entities = allProducts
                    .Where(p => p is HolidayPackage)
                    .Cast<HolidayPackage>();

                if (entities.Any())
                {
                    _context.HolidayPackages.AddRange(entities);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occurred while adding holiday packages.", ex);
            }
        }

        public async Task<IEnumerable<HolidayPackage>> GetProductsAsync()
        {
            try
            {
                return await _context.HolidayPackages.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occurred while retrieving holiday packages.", ex);
            }
        }
    }
}
