namespace gamershop.Server.Repositories;
using System;
using gamershop.Server.Database;
using System.Threading.Tasks;
using gamershop.Shared.Models;
using Dapper;



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
                        // Insert order into database
                        await connection.ExecuteAsync(
                            "INSERT INTO Orders (CustomerId) VALUES (@CustomerId)",
                            new { order.CustomerId },
                            transaction);

                        // Insert order items into database
                        foreach (var product in order.Items)
                        {
                            await connection.ExecuteAsync(
                                "INSERT INTO OrderItems (OrderId, ProductId) VALUES (@OrderId, @ProductId)",
                                new { OrderId = order.OrderId, ProductId = product.ProductId },
                                transaction);
                        }

                        // Commit transaction
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
    }

