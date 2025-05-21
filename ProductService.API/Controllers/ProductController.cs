using Core.Application.DTOs;
using Core.Application.Features.Products.Commands.SyncProducts;
using Core.Application.Features.Products.Queries.GetAllProducts;
using Core.Application.Features.Products.Queries.GetHolidayPackages;
using Core.Application.Features.Products.Queries.GetTourPackages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _mediator.Send(new GetProductsQuery());
            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }
            
            return Ok(products);
        }

        [HttpGet("holiday-packages")]
        public async Task<ActionResult<IEnumerable<HolidayPackageDto>>> GetHotels()
        {
            var hotels = await _mediator.Send(new GetHolidayPackagesQuery());

            if (hotels == null || !hotels.Any())
            {
                return NotFound("No hotels found.");
            }
            
            return Ok(hotels);
        }

        [HttpGet("tour-packages")]
        public async Task<ActionResult<IEnumerable<TourPackageDto>>> GetTours()
        {
            var tours = await _mediator.Send(new GetTourPackagesQuery());
            
            if (tours == null || !tours.Any())
            {
                return NotFound("No tours found.");
            }
            
            return Ok(tours);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncProducts()
        {
            try
            {
                var result = await _mediator.Send(new SyncProductsCommand());
                
                return Ok($"Successfully synced {result.TotalCount} products. (Holiday packages: {result.HolidayCount}, Tour packages: {result.TourCount})");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error syncing products: {ex.Message}");
            }
        }
    }
}
