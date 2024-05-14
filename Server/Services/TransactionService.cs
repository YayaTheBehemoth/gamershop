using gamershop.Shared.Models;
using System;
using System.Threading.Tasks;
using gamershop.Server.Repositories;

namespace gamershop.Server.Services
{
    public class TransactionService
    {
        private readonly SimpleMessageQueue<Order> _messageQueue;
        private readonly TransactionRepository _transactionRepository;

        public TransactionService(SimpleMessageQueue<Order> messageQueue, TransactionRepository transactionRepository)
        {
            _messageQueue = messageQueue;
            _transactionRepository = transactionRepository;
        }

        public async void ProcessOrders()
        {
            while (true)
            {
                var order = _messageQueue.Dequeue(); // Dequeue an order message

                // Calculate total price
                double amount = CalculateTotalPrice(order.Items);

                // Create transaction record
                var transaction = new Transaction
                {
                    OrderId = order.OrderId,
                    Amount = amount,
                    Date = DateTime.UtcNow
                };

                // Save transaction to database
              await _transactionRepository.InsertTransaction(transaction);

                // Log transaction
                Console.WriteLine($"Transaction processed for order {order.OrderId}. Amount: {amount}");
            }
        }

        private double CalculateTotalPrice(List<Product> products)
        {
            double totalPrice = 0;
            foreach (var product in products)
            {
                totalPrice += product.Price;
            }
            return totalPrice;
        }
    }
}
