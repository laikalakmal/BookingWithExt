using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Concreate
{
    public class CustomProductRepository : IProductRepository<CustomProduct>
    {
        private readonly AppDbContext _context;

        public CustomProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProductsAsync(List<CustomProduct> products)
        {
            try
            {
                if (products?.Any() != true)
                    return;

                var entities = products
                    .Where(p => p is CustomProduct)
                    .Cast<CustomProduct>();

                if (entities.Any())
                {
                    _context.CustomProducts.AddRange(entities);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding custom products.", ex);
            }
        }


        public async Task<CustomProduct> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.CustomProducts.FirstOrDefaultAsync(p => p.Id == id)
                    ?? throw new Exception("Custom product not found");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving the custom product.", ex);
            }
        }

        public async Task<IEnumerable<CustomProduct>> GetProductsAsync()
        {
            try
            {
                return await _context.CustomProducts.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving custom products.", ex);
            }
        }

        public async Task<bool> UpdateProduct(CustomProduct product)
        {
            try
            {
                var existing = await _context.CustomProducts.FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existing == null)
                    return false;

                _context.Entry(existing).CurrentValues.SetValues(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the custom product.", ex);
            }
        }
        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _context.CustomProducts.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            else
            {
                _context.CustomProducts.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}


