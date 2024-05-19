using gamershop.Server.Repositories;
using gamershop.Server.Services.Interfaces;
using System.Threading.Tasks;

namespace gamershop.Server.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentRepository _paymentRepository;

        public PaymentService(PaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<bool> ProcessPayment(double amount, string accountNumber)
        {
            // Check if the payment amount is valid
            if (amount <= 0)
            {
                return false; // Invalid payment amount
            }

            // Check if the account number is provided
            if (string.IsNullOrEmpty(accountNumber))
            {
                return false; // Account number is required
            }

            // Process the payment in the repository
            return await _paymentRepository.ProcessPayment(amount, accountNumber);
        }
    }
}
