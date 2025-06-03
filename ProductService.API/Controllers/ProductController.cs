using Core.Application.DTOs;
using Core.Application.Features.Products.Commands.AddProduct;
using Core.Application.Features.Products.Commands.PurchaseProduct;
using Core.Application.Features.Products.Commands.SyncProducts;
using Core.Application.Features.Products.Queries.Availability;
using Core.Application.Features.Products.Queries.GetAllProducts;
using Core.Application.Features.Products.Queries.GetHolidayPackages;
using Core.Application.Features.Products.Queries.GetTourPackages;
using Core.Domain.Entities.SupportClasses;
using Core.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

        [HttpPost("sync")]
        public async Task<IActionResult> SyncProducts()
        {
            try
            {
                var result = await _mediator.Send(new SyncProductsCommand());
                return Ok($"Successfully synced {result.TotalCount} products. (Holiday packages: {result.HolidayCount}, Tour packages: {result.TourCount})");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error syncing products.");
            }
        }

        [HttpGet("get")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts(
            [FromQuery][EnumDataType(typeof(ProductCategory))] ProductCategory? category = null,
            [FromQuery] string? externalId = null,
            [FromQuery] string? provider = null)
        {
            try
            {
                GetProductsQuery query;
                if (category == null && string.IsNullOrEmpty(externalId) && string.IsNullOrEmpty(provider))
                {
                    query = new GetProductsQuery();
                }
                else if (category == null)
                {
                    query = new GetProductsQuery(externalId ?? string.Empty, provider ?? string.Empty);
                }
                else
                {
                    query = new GetProductsQuery(category.Value, externalId ?? string.Empty, provider ?? string.Empty);
                }

                var products = await _mediator.Send(query);
                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }
                return Ok(products);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while searching for products.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            try
            {
                var product = await _mediator.Send(new GetProductByIdQuery(id));
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the product." + ex.Message);
            }
        }

        [HttpGet("holiday-packages")]
        public async Task<ActionResult<IEnumerable<HolidayPackageDto>>> GetHotels()
        {
            try
            {
                var hotels = await _mediator.Send(new GetHolidayPackagesQuery());
                if (hotels == null || !hotels.Any())
                {
                    return NotFound("No hotels found.");
                }
                return Ok(hotels);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving holiday packages.");
            }
        }

        [HttpGet("tour-packages")]
        public async Task<ActionResult<IEnumerable<TourPackageDto>>> GetTours()
        {
            try
            {
                var tours = await _mediator.Send(new GetTourPackagesQuery());
                if (tours == null || !tours.Any())
                {
                    return NotFound("No tours found.");
                }
                return Ok(tours);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving tour packages.");
            }
        }

        [HttpGet("{id}/availability")]
        public async Task<ActionResult<AvailabilityInfo>> GetProductAvailability(Guid id)
        {
            try
            {
                //ToDo: add real-time availability check via external api.
                var availabilityResult = await _mediator.Send(new CheckAvailabilityQuery(id));
                if (availabilityResult == null)
                {
                    return NotFound($"Availability for product with ID {id} not found.");
                }
                return Ok(availabilityResult);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving product availability.");
            }
        }

        [HttpPost("{id}/purchase")]
        public async Task<IActionResult> PurchaseProduct(Guid id, [FromBody] PurchaseRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid purchase request.");
            }
            try
            {
                PurchaseResponseDto purchaseResponseDto = await _mediator.Send(new PurchaseProductCommand { ProductId = id, Quantity = request.Quantity });
                return Ok(purchaseResponseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the purchase." +ex);
            }
        }

        [HttpPost("add")]
        public async Task<ActionResult<Guid>> AddProduct([FromBody] AddProductRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid product data");
            }

            try
            {
                var newProductId = await _mediator.Send(new AddProductCommand(request));
                return Ok(newProductId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while adding the product: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteProductCommand(id));
                if (result==true)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, $"Product with ID {id} not found.");
                }
                 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"An error occurred while deleting the product: {ex.Message}");
            }
        }
    }
}
