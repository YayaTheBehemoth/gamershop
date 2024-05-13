namespace gamershop.Shared.Models;
public class Customer 
{
    public int CustomerId {get; set;}

    public string FirstName {get; set;}

    public string LastName {get; set;}

    public string Email {get; set;}

    public string Phone {get; set;}

    public Address Address {get; set;}

    public int? UserId {get; set;}

}