using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class OrderItemValidation : AbstractValidator<OrderItem>
    {
        public OrderItemValidation()
        {
            RuleFor(oi => oi.ProductQuantity)
                .GreaterThan(0).WithMessage("Product quantity must be greater than zero.");
            RuleFor(oi => oi.QuantityWisePrice)
                .GreaterThan(0).WithMessage("Quantity wise price must be greater than zero.");
        }
    }
}
