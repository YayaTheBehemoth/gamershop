namespace gamershop.Shared.Models;
public class User 
{
    public int UserId {get; set;}
    public string UserName {get; set;}

    public HashedPassword StoredPassword {get; set;}

    public List<Order> Orderhistory {get; set;}

    public List<Product> Wishlist {get; set;}
}

public class HashedPassword
{
    public string Password {get; set;}
    public string Salt {get; set;}
}