using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.Models;

namespace PaymentService.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentRepository(PaymentDbContext context) => _context = context;

        public async Task<Payment?> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken)
        {
            return await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        }

        public async Task<Payment?> GetByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken)
        {
            return await _context.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, cancellationToken);
        }

        public async Task<Payment> CreateAsync(Payment payment, CancellationToken cancellationToken)
        {
            _context.Payments.Add(payment);

            await _context.SaveChangesAsync(cancellationToken);

            return payment;
        }
    }
}