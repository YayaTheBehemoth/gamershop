using System.Threading.Tasks;

namespace gamershop.Server.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPayment(double amount, string accountNumber);
    }
}
