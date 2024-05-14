using System;
using System.Threading.Tasks;
using Dapper;
using gamershop.Server.Database;
using gamershop.Shared.Models;

namespace gamershop.Server.Repositories
{
    public class OrderRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public OrderRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InsertOrder(Order order)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Insert order into database and retrieve the generated OrderId
                        var orderId = await connection.ExecuteScalarAsync<int>(
                            "INSERT INTO Orders (CustomerId) VALUES (@CustomerId) RETURNING Id",
                            new { order.CustomerId },
                            transaction);

                        // Associate the generated OrderId with the order object
                        order.OrderId = orderId;

                        // Commit the first transaction to persist the order
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        // Rollback transaction on failure
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }

            try
            {
                // Insert order items into a new transaction
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // Insert order items into database
                            foreach (var product in order.Items)
                            {
                                await connection.ExecuteAsync(
                                    "INSERT INTO Orderproduct (OrderId, ProductId) VALUES (@OrderId, @ProductId)",
                                    new { order.OrderId, product.ProductId },
                                    transaction);
                            }

                            // Commit the second transaction
                            await transaction.CommitAsync();
                        }
                        catch (Exception)
                        {
                            // Rollback transaction on failure
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // If an exception occurs while inserting order products, roll back the previous order insertion
                using (var connection = _connectionFactory.CreateConnection())
                {
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        await transaction.RollbackAsync();
                    }
                }
                throw;
            }
        }
    }
}
