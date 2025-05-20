using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Mappings
{
    public interface  IProductMapper
    {
        public static ProductDto FromDomain(Product product) {
            throw new NotImplementedException();
        }
        public static Product ToDomain(ProductDto productDto) {
            throw new NotImplementedException();
        }
    }
}