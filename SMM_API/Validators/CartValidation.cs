 using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class CartValidation : AbstractValidator<Cart>
    {
        public CartValidation()
        {
            RuleFor(c => c.ProductQuantity)
                .GreaterThan(0)
                .WithMessage("Product quantity must be greater than zero.");
            RuleFor(c => c.CheckoutDate)
                .NotEmpty()
                .When(c => c.IsCheckedOut)
                .WithMessage("Checkout date cannot be empty when the cart is checked out.");
            RuleFor(c => c.SessionId)
                .NotEmpty()
                .WithMessage("Session ID cannot be empty.")
                .MaximumLength(100)
                .WithMessage("Session ID cannot exceed 100 characters.");
        }
    }
}
