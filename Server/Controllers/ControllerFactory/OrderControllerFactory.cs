using Microsoft.Extensions.Configuration;
using gamershop.Server.Services;
using gamershop.Server.Repositories;
using gamershop.Server.Database;
using System.Collections.Generic;
using gamershop.Shared.Models;
using Microsoft.Extensions.Logging; // Add this namespace

namespace gamershop.Server.Controllers
{
    public class OrderControllerFactory
    {
        private List<OrderController> _orderControllers;
        private int _currentIndex;

        public OrderControllerFactory(int numberOfInstances, IConfiguration configuration)
        {
            // Create dependencies
            var dbConnectionFactory = new DbConnectionFactory(configuration);
            var orderRepository = new OrderRepository(dbConnectionFactory);
            var messageQueue = new SimpleMessageQueue<(Order, string, double)>();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole(); // Add console logger
            });
            var logger = loggerFactory.CreateLogger<OrderController>(); // Create logger instance

            // Create OrderService instance with the required dependencies
            var orderService = new OrderService(orderRepository, messageQueue);

            // Create OrderController instances with the OrderService dependency and logger
            _orderControllers = new List<OrderController>();
            for (int i = 0; i < numberOfInstances; i++)
            {
                var orderController = new OrderController(orderService, logger);
                orderController.InstanceId = (i + 1); // Set the InstanceId
                _orderControllers.Add(orderController);
            }

            _currentIndex = 0;
        }

public OrderController GetNextInstance()
{
    lock (_orderControllers) // Ensure thread safety
    {
        var nextInstance = _orderControllers[_currentIndex];
        _currentIndex = (_currentIndex + 1) % _orderControllers.Count;
        return nextInstance;
    }
}

        // Method to determine the instance ID (using round-robin approach)
        public int DetermineInstanceId()
        {
            lock (_orderControllers) // Ensure thread safety
            {
                var currentInstanceIndex = _currentIndex;
                _currentIndex = (_currentIndex + 1) % _orderControllers.Count;
                return currentInstanceIndex + 1; // Adding 1 to make it 1-based index
            }
        }
    }
}
