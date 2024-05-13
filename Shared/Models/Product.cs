namespace gamershop.Shared.Models;
public class Product
{
    public string ProductId {get; set;}

    public string ProductName {get; set;}

    public double Price {get; set;}

    public string Description{get; set;}

    public ProductCategory Category{get; set;}
    
}