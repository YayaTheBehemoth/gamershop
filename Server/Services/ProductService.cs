using AutoMapper;
using gamershop.Server.Repositories.Interfaces;
using gamershop.Server.Services.Interfaces;
using gamershop.Shared.DTOs;
using gamershop.Shared.Models;

namespace gamershop.Server.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            var categories = await _productRepository.GetProductCategoriesAsync();

            // Map products to ProductDTOs and populate the Category property
            return products.Select(product =>
            {
                var productDTO = _mapper.Map<ProductDTO>(product);
                productDTO.Category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);
                return productDTO;
            });
        }
/*
public async Task<ProductDTO> GetProductByIdAsync(int productId)
{
    var product = await _productRepository.GetProductByIdAsync(productId);
    if (product == null)
    {
        // Handle null product
        return null;
    }

    // Retrieve all categories
    var categories = await _productRepository.GetProductCategoriesAsync();

    // Find the category for the product
    var category = categories.FirstOrDefault(c => c.CategoryId == product.CategoryId);

    // Map product to ProductDTO and populate the Category property
    var productDTO = _mapper.Map<ProductDTO>(product);
    productDTO.Category = category != null ? _mapper.Map<ProductCategoryDTO>(category) : null;

    return productDTO;
}

*/
  

        public async Task AddProductAsync(ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            await _productRepository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }
    }
}
