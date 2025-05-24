using Core.Application.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Concreate
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
            try
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
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occurred while adding tour packages.", ex);
            }
        }

        public async Task<IEnumerable<TourPackage>> GetProductsAsync()
        {
            try
            {
                return await _context.TourPackages.ToListAsync();
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occurred while retrieving tour packages.", ex);
            }
        }
    }
}