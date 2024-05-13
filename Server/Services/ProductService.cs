using gamershop.Shared.Models;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using gamershop.Server.Services.Interface;

namespace gamershop.Server.Services
{
    public class ProductService : IProductService
    {
        private readonly string _connectionString;

        public ProductService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }


        // GET all products query
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var products = await connection.QueryAsync<Product>("SELECT * FROM Products");
                return products;
            }
        }

        // Get product by id query
        public async Task<Product> GetProductByIdAsync(string productId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var product = await connection.QueryFirstOrDefaultAsync<Product>("SELECT * FROM Products WHERE ProductId = @ProductId", new { ProductId = productId});
                return product;
            }
        }

        // Add product query
        public async Task AddProductAsync(Product product)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var sql = "INSERT INTO Products (ProductId, ProductName, Price, Description, Category) VALUES (@ProductId, @ProductName, @Price, @Description, @Category)";
                await connection.ExecuteAsync(sql, product);
            }
        }

        // Update product query
        public async Task UpdateProductAsync(Product product)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var sql = "UPDATE Products SET ProductName = @ProductName, Price = @Price, Description = @Description, Category = @Category WHERE ProductId = @ProductId";
                await connection.ExecuteAsync(sql, product);
            }
        }

        // Delete product query
        public async Task DeleteProductAsync(string productId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Products WHERE ProductId = @ProductId";
                await connection.ExecuteAsync(sql, new { ProductId = productId });
            }
        }


    }
}

