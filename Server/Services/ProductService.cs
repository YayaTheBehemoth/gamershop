using AutoMapper;
using gamershop.Server.Repositories.Interfaces;
using gamershop.Server.Services.Interfaces;
using gamershop.Shared.DTOs;
using gamershop.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gamershop.Server.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IMemoryCache _cache;
        private const string AllProductsCacheKey = "AllProducts";

        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger, IMemoryCache cache)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

     public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
{
    if (!_cache.TryGetValue<IEnumerable<ProductDTO>>(AllProductsCacheKey, out var products))
    {
        _logger.LogInformation("Cache miss: AllProducts");
        products = await RetrieveProductsFromRepository();
        // Cache the products with a sliding expiration of 1 hour
        _cache.Set(AllProductsCacheKey, products, TimeSpan.FromHours(1));
    }
    else
    {
        _logger.LogInformation("Cache hit: AllProducts");
    }
    return products;
}

        private async Task<IEnumerable<ProductDTO>> RetrieveProductsFromRepository()
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

