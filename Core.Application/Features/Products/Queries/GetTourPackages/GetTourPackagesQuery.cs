using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetTourPackages
{
    public class GetTourPackagesQuery : IRequest<List<TourPackageDto>>
    {
    }
}