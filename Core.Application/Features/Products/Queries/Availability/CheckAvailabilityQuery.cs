using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Products.Queries.Availability
{
    public class CheckAvailabilityQuery:IRequest<CheckAvailabilityResult>
    {
       public Guid ProductId { get; set; }
        public CheckAvailabilityQuery(Guid productId)
        {
            ProductId = productId;
        }

    }
}
