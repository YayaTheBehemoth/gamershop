namespace gamershop.Server.Repositories;
using System;

using System.Threading.Tasks;
using gamershop.Shared.Models;

using gamershop.Server.Database;
using Dapper;

public class TransactionRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TransactionRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InsertTransaction(Transaction transaction)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var transactionScope = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Insert transaction into database
                        await connection.ExecuteAsync(
                            "INSERT INTO Transactions (OrderId, Amount, Date) VALUES (@OrderId, @Amount, @Date)",
                            new { transaction.OrderId, transaction.Amount, transaction.Date },
                            transactionScope);

                        // Commit transaction
                        await transactionScope.CommitAsync();
                    }
                    catch (Exception)
                    {
                        // Rollback transaction on failure
                        await transactionScope.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }

