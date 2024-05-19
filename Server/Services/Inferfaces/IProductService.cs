using gamershop.Shared.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gamershop.Server.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        //Task<ProductDTO> GetProductByIdAsync(int productId);
        Task AddProductAsync(ProductDTO productDTO);
        Task UpdateProductAsync(ProductDTO productDTO);
        Task DeleteProductAsync(int productId);
    }
}
