using gamershop.Shared.Models;
namespace gamershop.Shared.DTOs;
 public class PlaceOrderRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Product> Products { get; set; }
        public string AccountNumber { get; set; }
    }