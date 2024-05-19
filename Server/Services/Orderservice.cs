using System;
using System.Collections.Generic;
using gamershop.Shared.Models;
using gamershop.Server.Repositories;

namespace gamershop.Server.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly SimpleMessageQueue<(Order, string, double)> _messageQueue;

        public OrderService(OrderRepository orderRepository, SimpleMessageQueue<(Order, string, double)> messageQueue)
        {
            _orderRepository = orderRepository;
            _messageQueue = messageQueue;
        }

        public async void PlaceOrder(string firstName, string lastName, string email, List<Product> products, string accountNumber)
        {
            // Validate inputs
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email) || products == null || products.Count == 0 || string.IsNullOrEmpty(accountNumber))
            {
                throw new ArgumentException("Invalid customer information, product list, or account number");
            }

            // Calculate total price
            double totalPrice = CalculateTotalPrice(products);

            // Create order
            var order = new Order
            {
                Items = products
            };

            // Insert order into repository
            await _orderRepository.InsertOrder(order, firstName, lastName, email);

            // Enqueue order message with account number and total price
            _messageQueue.Enqueue((order, accountNumber, totalPrice));

            // Log order placement
            Console.WriteLine($"Order placed for customer {firstName} {lastName}. Total: {totalPrice}");
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
