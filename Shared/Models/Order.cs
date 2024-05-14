using System.Collections.Generic;
namespace gamershop.Shared.Models;
public class Order 
{
    public int OrderId {get; set;}

    public int CustomerId {get; set;}

    public List<Product> Items {get; set;} 


   
    
}