using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductService<Product, ProductDto> _productService;

    public DeleteProductCommandHandler(IProductService<Product, ProductDto> productService)
    {
        _productService = productService;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            return await _productService.DeleteProductAsync(request.Id);
        }
        catch (KeyNotFoundException)
        {

            return false;
        }
    }
}