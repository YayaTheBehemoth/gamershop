using Microsoft.AspNetCore.Mvc;
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
        public int InstanceId { get; private set; } // Property to hold the InstanceId

        // Constructor without the InstanceId parameter
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // Method to set the InstanceId
        public void SetInstanceId(int instanceId)
        {
            InstanceId = instanceId;
        }

      [HttpPost("PlaceOrder/{instanceId}")]
public IActionResult PlaceOrder(int instanceId, [FromBody] PlaceOrderRequest request)
{
    try
    {
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
