using VeridianClimatePulse.Common.Models;
using VeridianClimatePulse.Dtos.PaymentDto;

namespace VeridianClimatePulse.IServices
{
    public interface IPaymentService
    {
        Task<ResultResponseDto<CheckoutSessionResponse>> CreateCheckoutSession(CreateCheckoutSessionDto request);
        Task<ResultResponseDto<VerifySessionResponse>> VerifySession(VerifySessionDto request);
        Task<ResultResponseDto<string>> StripeWebhook();
    }
}
