using gamershop.Server.Services;
using gamershop.Shared.Models;
using gamershop.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace gamershop.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                // Call the service with the new parameters
                _orderService.PlaceOrder(request.FirstName, request.LastName, request.Email, request.Products, request.AccountNumber);
                return Ok("Order placed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

    // Define a model for the request body
   
}
