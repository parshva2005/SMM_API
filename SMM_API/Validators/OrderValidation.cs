using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class OrderValidation : AbstractValidator<Order>
    {
        public OrderValidation()
        {
            RuleFor(o => o.OrderNumber)
                .NotEmpty().WithMessage("Order number cannot be empty.")
                .Length(1, 50).WithMessage("Order number must be between 1 and 50 characters.");
            RuleFor(o => o.Status)
                .NotEmpty().WithMessage("Order status cannot be empty.")
                .Length(1, 20).WithMessage("Order status must be between 1 and 20 characters.");
            RuleFor(o => o.TaxAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Tax amount cannot be negative.");
            RuleFor(o => o.DiscountAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount amount cannot be negative.");
            RuleFor(o => o.PaymentMethod)
                .Length(0, 50).WithMessage("Payment method must be up to 50 characters long.")
                .When(o => !string.IsNullOrEmpty(o.PaymentMethod));
            RuleFor(o => o.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.")
                .When(o => !string.IsNullOrEmpty(o.Notes));
            RuleFor(o => o.TotalPrice)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");
        }
    }
}
