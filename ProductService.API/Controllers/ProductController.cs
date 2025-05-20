using Core.Application.Services;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DbContext _context;
        private readonly IProductService _productService;


        public ProductController(AppDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;

        }



        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }
            return Ok(products);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncProducts()
        {
            try
            {
                var count = await _productService.SyncProductsFromExternalAsync();
                return Ok($"Successfully synced {count} products.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error syncing products: {ex.Message}");
            }
        }

    }
}
