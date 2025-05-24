using MediatR;

namespace Core.Application.Features.Products.Commands.SyncProducts
{
    public class SyncProductsCommand : IRequest<SyncProductsResponse>
    {
    }

    public class SyncProductsResponse
    {
        public int HolidayCount { get; set; }
        public int TourCount { get; set; }
        public int TotalCount => HolidayCount + TourCount;

        public List<string> errorMessages { get; set; } = new List<string>();
    }
}
