﻿using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.Features.Products.Commands.PurchaseProduct
{
    public class PurchaseProductCommandHandler : IRequestHandler<PurchaseProductCommand, PurchaseResponseDto>
    {


        private readonly IProductService _productService;



        public PurchaseProductCommandHandler(
            IProductService productService)

        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

        }

        public async Task<PurchaseResponseDto> Handle(PurchaseProductCommand request, CancellationToken cancellationToken)
        {
            try
            {

                ProductDto product = await _productService.GetByIdAsync(request.ProductId) ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found");
                var response = await _productService.PurchaseProductAsync(product, request.Quantity);
                return response;
            }
            catch (Exception ex)
            {

                throw new ApplicationException($"Failed to purchase product with ID {request.ProductId}", ex);
            }
        }
    }
}
