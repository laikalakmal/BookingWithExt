using Core.Domain.Entities;

namespace Core.Application.Interfaces
{

    // this is a factory interface for the product repository.
    // it is used to create the product repository for each product type.
    // it is used in the product service to create the product repository for each product type.
    // each implimentation should have a factory that creates the product repository.
    public interface IProductRepositoryFactory
    {
        bool CanHandle(Product product);
        IProductRepository<Product> CreateRepository();
    }
}