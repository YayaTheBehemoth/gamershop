using gamershop.Shared.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using gamershop.Server.Database;

namespace gamershop.Server.Repositories
{
    public class ProductRepository
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
                var products = await connection.QueryAsync<Product>("SELECT * FROM Products");
                return products;
            }
        }

        // Get product by id query
        public async Task<Product> GetProductByIdAsync(string productId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var product = await connection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Products WHERE ProductId = @ProductId", new { ProductId = productId });
                return product;
            }
        }

        // Add product query
        public async Task AddProductAsync(Product product)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "INSERT INTO Products (ProductId, ProductName, Price, Description, CategoryId) VALUES (@ProductId, @ProductName, @Price, @Description, @CategoryId)";
                await connection.ExecuteAsync(sql, product);
            }
        }

        // Update product query
        public async Task UpdateProductAsync(Product product)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "UPDATE Products SET ProductName = @ProductName, Price = @Price, Description = @Description, CategoryId = @CategoryId WHERE ProductId = @ProductId";
                await connection.ExecuteAsync(sql, product);
            }
        }

        // Delete product query
        public async Task DeleteProductAsync(string productId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                var sql = "DELETE FROM Products WHERE ProductId = @ProductId";
                await connection.ExecuteAsync(sql, new { ProductId = productId });
            }
        }
    }
}
