using Core.Application.Interfaces;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProductsAsync(List<Product> products)
        {
            try
            {
                if (products?.Any() != true)
                    return;

                var entities = products
                    .Where(p => p is Product)
                    .Cast<Product>();

                if (entities.Any())
                {
                    _context.Products.AddRange(entities);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while adding custom products.", ex);
            }
        }


        public async Task<Product> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Products.FirstOrDefaultAsync(p => p.Id == id)
                    ?? throw new Exception("Custom product not found");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving the custom product.", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving custom products.", ex);
            }
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            try
            {
                var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existing == null)
                    return false;

                _context.Entry(existing).CurrentValues.SetValues(product);

                // Manually update complex object
                existing.Availability = product.Availability;

                // Manually update collections
                existing.Attributes = new Dictionary<string, object>(product.Attributes);
                existing.ImageUrl = new List<string>(product.ImageUrl);

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
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            else
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteProductAsync(Product product)
        {
            try
            {
                if (product == null)
                {
                    throw new ArgumentNullException(nameof(product), "Product cannot be null.");
                }
                if (product is not Product Product)
                {
                    throw new InvalidOperationException("Product is not of type Product.");
                }
                _context.Products.Remove(Product);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the custom product.", ex);
            }

        }
    }
}


