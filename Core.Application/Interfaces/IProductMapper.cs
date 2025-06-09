using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IProductMapper
    {
        /// Converts a domain entity to a DTO
        /// impliment this for each product type.
        public static ProductDto FromDomain(Product product)
        {
            throw new NotImplementedException();
        }
        public static Product ToDomain(ProductDto productDto)
        {
            throw new NotImplementedException();
        }
    }

}

