using Microsoft.AspNetCore.Mvc;
using gamershop.Shared.Models;
using gamershop.Server.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using gamershop.Server.Services.Interface;

namespace gamershop.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
      
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProduct(string id, Product product)
        {
            if(id != product.ProductId)
            {
                return BadRequest();
            }

            var existingProduct = await _productService.GetProductByIdAsync(id);
            if(existingProduct == null)
            {
                return NotFound();
            }

            await _productService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(string id)
        {
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if(existingProduct == null)
            {
                return NotFound();
            }

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
