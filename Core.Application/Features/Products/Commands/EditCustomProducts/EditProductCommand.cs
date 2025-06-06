using Core.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Features.Products.Commands.EditCustomProducts
{
    public class EditProductCommand :IRequest<EditProductResponse>
    {

        public required  AddProductRequest productRequest { get; set; }
        public required Guid ProductId { get; set; }

}
}
