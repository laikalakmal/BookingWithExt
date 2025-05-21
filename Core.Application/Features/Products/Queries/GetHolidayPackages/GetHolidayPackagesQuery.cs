using Core.Application.DTOs;
using MediatR;

namespace Core.Application.Features.Products.Queries.GetHolidayPackages
{
    public class GetHolidayPackagesQuery : IRequest<List<HolidayPackageDto>>
    {
    }
}