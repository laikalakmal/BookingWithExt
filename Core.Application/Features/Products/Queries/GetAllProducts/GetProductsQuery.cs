using Core.Application.DTOs;
using Core.Domain.Entities;
using Core.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

