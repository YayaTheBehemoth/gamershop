using gamershop.Server.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using gamershop.Shared.Models;
using gamershop.Server.Repositories;

namespace gamershop.Server.Services
{
    public class TransactionService : BackgroundService
    {
        private readonly SimpleMessageQueue<(Order, string, double)> _messageQueue;
        private readonly TransactionRepository _transactionRepository;
        private readonly IPaymentService _paymentService;

        public TransactionService(SimpleMessageQueue<(Order, string, double)> messageQueue, TransactionRepository transactionRepository, IPaymentService paymentService)
        {
            _messageQueue = messageQueue;
            _transactionRepository = transactionRepository;
            _paymentService = paymentService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("Waiting for orders...");
                    var (order, accountNumber, totalPrice) = await Task.Run(() => _messageQueue.Dequeue(), stoppingToken);

                    Console.WriteLine("Order received. Processing payment...");
                    // Process payment
                    bool paymentSuccessful = await _paymentService.ProcessPayment(totalPrice, accountNumber);

                    if (paymentSuccessful)
                    {
                        // Create transaction record
                        var transaction = new Transaction
                        {
                            OrderId = order.OrderId,
                            Amount = totalPrice,
                            Date = DateTime.UtcNow
                        };

                        // Save transaction to database
                        await _transactionRepository.InsertTransaction(transaction);

                        // Log transaction
                        Console.WriteLine($"Transaction processed for order {order.OrderId}. Amount: {totalPrice}");
                    }
                    else
                    {
                        // Handle payment failure
                        Console.WriteLine($"Payment failed for order {order.OrderId}.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Log cancellation
                Console.WriteLine("Cancellation token triggered. Stopping TransactionService.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // You can add more detailed logging or error handling here as needed
            }
        }
    }
}
