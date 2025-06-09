using Core.Application.DTOs;

namespace Core.Application.Interfaces
{
    /// <summary>
    /// Defines editing capabilities for products that support modification after creation.
    /// This interface should only be implemented by services that handle editable product types.
    /// </summary>
    /// <typeparam name="T">The DTO type of the product, must inherit from ProductDto</typeparam>
    public interface IEditableProduct
    {
        /// <summary>
        /// Updates an existing product with new values
        /// </summary>
        /// <param name="id">The unique identifier of the product to edit</param>
        /// <param name="newProductDto">The updated product data</param>
        /// <returns>True if the product was successfully updated, false otherwise</returns>
        Task<bool> EditProduct(Guid id, ProductDto newProductDto);
    }
}
