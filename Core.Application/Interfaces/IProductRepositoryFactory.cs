using Core.Domain.Entities;

namespace Core.Application.Interfaces
{

    // this is a factory interface for the product repository.
    // it is used to create the product repository for each product type.
    // it is used in the product service to create the product repository for each product type.
    
    public interface IProductRepositoryFactory
    {
        bool CanHandle(Product product);
        IProductRepository<Product> CreateRepository();
    }
}