using System.Collections.Generic;
namespace gamershop.Shared.Models;
public class Order 
{
    public string OrderId {get; set;}

    public string CustomerId {get; set;}

    public List<Product> Items {get; set;} 


   
    
}