using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Application.Mappings;
using Core.Domain.Entities;

namespace Core.Application.Services
{
    public class ProductService:IProductService,IEditableProduct,IAddableProduct

    {
        private readonly IProductRepository _repository;
        private readonly IEnumerable<IExternalProductApiAdapter> _adapters;

        public ProductService(
            IProductRepository repository,
            IEnumerable<IExternalProductApiAdapter> adapters
            )
        {
            _adapters = adapters ?? throw new ArgumentNullException(nameof(adapters), "Adapters cannot be null.");
            _repository = repository ?? throw new ArgumentNullException(nameof(repository), "repository cannot be null");
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            try
            {
                var localProducts = await _repository.GetProductsAsync();
                if (localProducts == null)
                {
                    return [];
                }
                return localProducts.OfType<Product>().Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch custom products: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Product>> FetchExternalProductsAsync()
        {
            var allProducts = new List<Product>();


            foreach (var adapter in _adapters)
            {
                
                if (adapter == null)
                { 
                    continue;
                }

                try
                {
                    var productDtos = await adapter.FetchProductsAsync();

                    if (productDtos != null && productDtos.Any())
                    {
                        foreach (var dto in productDtos)
                        {
                            // Create a new Product from the ProductDto
                            var Product = new Product(
                                dto.ExternalId,
                                dto.Name,
                                dto.Price,
                                dto.Description,
                                dto.Category,
                                dto.Provider,
                                dto.Availability
                            )
                            {
                                Id = dto.Id,
                                ImageUrl = dto.ImageUrl ?? string.Empty,
                                CreatedAt = dto.CreatedAt,
                                UpdatedAt = dto.UpdatedAt,
                                Attributes = dto.Attributes
                            };

                            allProducts.Add(Product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching products from adapter {adapter.AdapterName}: {ex.Message}");
                }
            }

            return allProducts;
        }

        public async Task<int> SyncProductsFromExternalAsync()
        {
            try
            {

                var externalProducts = await FetchExternalProductsAsync();

                if (externalProducts == null || !externalProducts.Any())
                {
                    return 0;
                }

                // Get existing products from repository to check for updates
                var existingProducts = await _repository.GetProductsAsync();
                var existingProductsDict = existingProducts.ToDictionary(p => p.ExternalId ?? "", p => p);

                var productsToAdd = new List<Product>();
                var productsUpdated = 0;

                foreach (var externalProduct in externalProducts)
                {

                    if (string.IsNullOrEmpty(externalProduct.ExternalId))
                    {
                        continue;
                    }


                    if (existingProductsDict.TryGetValue(externalProduct.ExternalId, out var existingProduct))
                    {
                        // Update existing product properties
                        existingProduct.Name = externalProduct.Name;
                        existingProduct.Price = externalProduct.Price;
                        existingProduct.Description = externalProduct.Description;
                        existingProduct.Category = externalProduct.Category;
                        existingProduct.Provider = externalProduct.Provider;
                        existingProduct.Availability = externalProduct.Availability;
                        existingProduct.ImageUrl = externalProduct.ImageUrl;
                        existingProduct.UpdatedAt = DateTime.UtcNow;


                        await _repository.UpdateProduct(existingProduct);
                        productsUpdated++;
                    }
                    else
                    {
                        externalProduct.Id = Guid.NewGuid();
                        externalProduct.CreatedAt = DateTime.UtcNow;
                        externalProduct.UpdatedAt = DateTime.UtcNow;
                        productsToAdd.Add(externalProduct);
                    }
                }

                if (productsToAdd.Any())
                {
                    await _repository.AddProductsAsync(productsToAdd);
                }

                return productsToAdd.Count + productsUpdated;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to sync products from external sources: {ex.Message}", ex);
            }
        }

        public Product MapToDomain(ProductDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "ProductDto cannot be null.");

            return ProductMapper.ToDomain(dto);
        }

        public ProductDto MapToDto(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");

            return ProductMapper.FromDomain(product);
        }

        public async Task<ProductDto> GetByIdAsync(Guid id)
        {
            try
            {
                var product = await _repository.GetByIdAsync(id) ?? throw new Exception($"Product with ID {id} not found.");
                return MapToDto(product);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get custom product by ID: {ex.Message}", ex);
            }
        }

        public async Task<PurchaseResponseDto> PurchaseProductAsync(ProductDto product, int quantity)
        {
            try
            {
                Product Product = await _repository.GetByIdAsync(product.Id) ?? throw new Exception($"Custom product with ID {product.Id} not found.");
                if (quantity <= 0 || quantity > Product.Availability.RemainingSlots)
                {
                    throw new Exception($"Invalid quantity {quantity} for product {product.Name}. Available slots: {Product.Availability.RemainingSlots}");
                }



                // Update inventory
                if (Product != null)
                {
                    Product.Availability.RemainingSlots -= quantity;
                    await _repository.UpdateProduct(Product);
                    // Create a success response for internal custom products
                    var response = new PurchaseResponseDto(product.ExternalId)
                    {
                        TransactionId = Guid.NewGuid().ToString().Substring(8, 0),
                        IsSuccess = true,
                        ProductId = product.Id,
                        Quantity = quantity,
                        TotalAmount = product.Price.Amount * quantity,
                        CurrencyCode = product.Price.Currency.ToString(),
                        PurchaseDate = DateTime.UtcNow,
                        Provider = Product?.Provider ?? "BookWithExt",
                        ConfirmationCode = Guid.NewGuid().ToString().Substring(0, 8),
                        Message = "Purchase successful"
                    };

                    return response;

                }
                else
                {
                    throw new Exception($"Custom product with ID {product.Id} not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to process purchase: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            try
            {
                return await _repository.DeleteProductAsync(product);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete custom product: {ex.Message}", ex);
            }
        }


        public async Task<bool> EditProduct(Guid productId, ProductDto updatedProductDto)
        {
            ArgumentNullException.ThrowIfNull(updatedProductDto);

            try
            {

                var existingProduct = await _repository.GetByIdAsync(productId) ??
                    throw new KeyNotFoundException($"Product with ID {productId} not found.");


                var newProduct = MapToDomain(updatedProductDto);

                existingProduct.Name = newProduct.Name;
                existingProduct.Price = newProduct.Price;
                existingProduct.Description = newProduct.Description;
                existingProduct.Category = newProduct.Category;
                existingProduct.Provider = newProduct.Provider;
                existingProduct.Availability = newProduct.Availability;
                existingProduct.Attributes = newProduct.Attributes;
                existingProduct.ImageUrl = newProduct.ImageUrl ?? string.Empty;
                existingProduct.UpdatedAt = DateTime.UtcNow;



                // Save changes
                return await _repository.UpdateProduct(existingProduct);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to edit custom product: {ex.Message}", ex);
            }
        }

        public async Task<Guid> AddProductAsync(ProductDto productRequest)
        {
            ArgumentNullException.ThrowIfNull(productRequest);

            try
            {

                var product = MapToDomain(productRequest);


                product.Id = Guid.NewGuid();
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;

                await _repository.AddProductsAsync(new List<Product> { product });

                return product.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add custom product: {ex.Message}", ex);
            }
        }
    }
}
