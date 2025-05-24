using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IProductMapper<TDomain, TDto> where TDomain : Product where TDto : ProductDto
    {
        /// Converts a domain entity to a DTO
        /// impliment this for each product type.
        public static TDto FromDomain(TDomain product)
        {
            throw new NotImplementedException();
        }
        public static TDomain ToDomain(TDto productDto)
        {
            throw new NotImplementedException();
        }
    }

}

