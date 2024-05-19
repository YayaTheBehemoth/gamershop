using gamershop.Shared.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using gamershop.Server.Database;
using gamershop.Server.Services.Interface;

namespace gamershop.Server.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ProductRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // GET all products query
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var products = await connection.QueryAsync<Product>("SELECT * FROM product");
                return products;
            }
        }

        // Get product by id query
        public async Task<Product> GetProductByIdAsync(int productId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var product = await connection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM product WHERE productid = @productid", new { ProductId = productId });
                return product;
            }
        }

        // Add product query
        public async Task AddProductAsync(Product product)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "INSERT INTO product (productid, productname, price, description, categoryid) VALUES (@productid, @productname, @price, @description, @categoryid)";
                await connection.ExecuteAsync(sql, new
                {
                    product.ProductId,
                    product.ProductName,
                    product.Price,
                    product.Description,
                    CategoryId = product.Category.CategoryId
                });
            }
        }

        // Update product query
        public async Task UpdateProductAsync(Product product)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE product SET productname = @productname, price = @price, description = @description, categoryid = @categoryid WHERE productid = @productid";
                await connection.ExecuteAsync(sql, new
                {
                    product.ProductName,
                    product.Price,
                    product.Description,
                    CategoryId = product.Category.CategoryId,
                    product.ProductId
                });
            }
        }

        // Delete product query
        public async Task DeleteProductAsync(int productId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM product WHERE productId = @productId";
                await connection.ExecuteAsync(sql, new { ProductId = productId });
            }
        }
    }
}