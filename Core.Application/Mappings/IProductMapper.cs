using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Mappings
{
    public interface IProductMapper<TDomain, TDto> where TDomain : Product where TDto : ProductDto
    {
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