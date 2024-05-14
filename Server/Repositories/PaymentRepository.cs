using System;
using System.Threading.Tasks;
using Dapper;
using gamershop.Server.Database;

namespace gamershop.Server.Repositories
{
    public class PaymentRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PaymentRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ProcessPayment(double amount, string accountNumber)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                try
                {
                    // Retrieve the account balance
                    var balance = await connection.QueryFirstOrDefaultAsync<double>(
                        "SELECT Balance FROM BankAccount WHERE AccountNumber = @AccountNumber",
                        new { AccountNumber = accountNumber });

                    // Check if the account exists and has sufficient balance
                    if (balance >= amount)
                    {
                        // Deduct the payment amount from the account balance
                        balance -= amount;

                        // Update the account balance in the database
                        await connection.ExecuteAsync(
                            "UPDATE BankAccount SET Balance = @Balance WHERE AccountNumber = @AccountNumber",
                            new { Balance = balance, AccountNumber = accountNumber });

                        // Payment processed successfully
                        return true;
                    }
                    else
                    {
                        // Insufficient balance
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing payment: {ex.Message}");
                    return false;
                }
            }
        }
    }
}
