using VeridianClimatePulse.Enums;

namespace VeridianClimatePulse.Dtos.PaymentDto
{
    public class CreateCheckoutSessionDto
    {
        public int UserID { get; set; }
        public TieredAccessPlan Tier { get; set; }
        public int Amount { get; set; }
    }
}
