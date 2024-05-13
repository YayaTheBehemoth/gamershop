namespace gamershop.Shared.Models;
public class Transaction 
{
    public int TransactionId {get; set;}

    public int OrderId {get; set;}

    public double Amount {get; set;}
    
    public DateTime Date {get; set;}

    
}