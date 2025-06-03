using Core.Application.Interfaces;
using Core.Domain.Entities;
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

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _context.TourPackages.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                    return await Task.FromResult(false);

                _context.TourPackages.Remove(product);
                return await _context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occured while deleting tour package", ex);
            }
        }

        public async Task<TourPackage> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.TourPackages.FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("Product not found");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving the tour package.", ex);
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


        public async Task<bool> UpdateProduct(TourPackage tourPackage)
        {
            try
            {
                var existing = await _context.TourPackages.FirstOrDefaultAsync(p => p.Id == tourPackage.Id);
                if (existing == null)
                    return false;

                _context.Entry(existing).CurrentValues.SetValues(tourPackage);



                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the tour package.", ex);
            }
        }

    }
}