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

        public async Task<int> GetCustomerId(string firstName, string lastName, string email)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                // Check if the customer already exists
                var existingCustomerId = await connection.ExecuteScalarAsync<int?>(
                    "SELECT CustomerId FROM Customer WHERE FirstName = @FirstName AND LastName = @LastName AND Email = @Email",
                    new { FirstName = firstName, LastName = lastName, Email = email });

                if (existingCustomerId.HasValue)
                {
                    // Customer already exists, return the CustomerId
                    return existingCustomerId.Value;
                }
                else
                {
                    // Customer doesn't exist, insert a new customer
                    var customerId = await connection.ExecuteScalarAsync<int>(
                        "INSERT INTO Customer (FirstName, LastName, Email) VALUES (@FirstName, @LastName, @Email) RETURNING CustomerId",
                        new { FirstName = firstName, LastName = lastName, Email = email });

                    return customerId;
                }
            }
        }

        public async Task InsertOrder(Order order, string firstname,string lastname, string email)
        {
            // Get or insert the CustomerId
            order.CustomerId = await GetCustomerId(firstname,lastname, email);

            // Insert the order
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

                        // Commit the transaction to persist the order
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

            // Insert order items
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
                                "INSERT INTO OrderProduct (OrderId, ProductId) VALUES (@orderid, @productid)",
                                new { order.OrderId, product.ProductId },
                                transaction);
                        }

                        // Commit the transaction
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
}
