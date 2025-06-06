using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

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

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                var product = await _context.HolidayPackages.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                    return false;
                 _context.HolidayPackages.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            } catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the holiday package.", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(HolidayPackage product)
        {
            try
            {
                if (product == null)
                    throw new ArgumentNullException(nameof(product), "Product cannot be null.");
                if (product is not HolidayPackage)
                    throw new ArgumentException("Product must be of type HolidayPackage.", nameof(product));

                _context.HolidayPackages.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException("An error occurred while deleting the holiday package.", ex);
            }


        }

        public async Task<HolidayPackage> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.HolidayPackages.FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("Product not found");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving the holiday package.", ex);
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

        public async Task<bool> UpdateProduct(HolidayPackage holidayPackage)
        {
            try
            {
                var existing = await _context.HolidayPackages.FirstOrDefaultAsync(p => p.Id == holidayPackage.Id);
                if (existing == null)
                    return false;

                _context.Entry(existing).CurrentValues.SetValues(holidayPackage);



                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the holiday package.", ex);
            }
        }
    }
}
