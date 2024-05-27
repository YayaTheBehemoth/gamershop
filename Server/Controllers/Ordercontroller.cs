using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Add this namespace
using gamershop.Server.Services;
using gamershop.Shared.DTOs;
using System;

namespace gamershop.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger; // Add ILogger<OrderController> field

        public int InstanceId { get;  set; } // Property to hold the InstanceId

        // Modify the constructor to inject ILogger<OrderController>
        public OrderController(OrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // Method to set the InstanceId
      

        [HttpPost("PlaceOrder/{instanceId}")]
        public IActionResult PlaceOrder(int instanceId, [FromBody] PlaceOrderRequest request)
        {
            try
            {
                // Log the instance being used
                _logger.LogInformation($"Processing order on instance {instanceId}");

                // Call the service with the new parameters
                _orderService.PlaceOrder(request.FirstName, request.LastName, request.Email, request.Products, request.AccountNumber);
                
               

                // Optionally, you can use the instanceId in your logic here

                return Ok("Order placed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("GetInstanceId")]
        public IActionResult GetInstanceId()
        {
            try
            {
                return Ok(InstanceId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
