using Core.Application.DTOs;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Products.Queries.GetAllProducts
{
    public class GetProductsQuery:IRequest<List<ProductDto>>
    {
    }
}
