using Core.Application.DTOs;
using Core.Domain.Enums;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetAllProducts
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public GetProductsQuery()
        {
        }

        public GetProductsQuery(string externalId, string provider)
        {
            ExternalId = externalId;
            Provider = provider;
        }

        public GetProductsQuery(ProductCategory category, string externalId, string provider)
        {
            Category = category;
            ExternalId = externalId;
            Provider = provider;
        }

        public string? ExternalId { get; }
        public string? Provider { get; }
        public ProductCategory? Category { get; }
    }
}

