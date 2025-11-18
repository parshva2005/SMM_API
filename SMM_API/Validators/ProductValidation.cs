using FluentValidation;
using SMM_API.Models;
using SUPER_MARKET_MANAGEMENT_API.Controllers;

namespace SMM_API.Validators
{
    public class ProductValidation : AbstractValidator<Product>
    {
        public ProductValidation()
        {
            RuleFor(p => p.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(20).WithMessage("Product name cannot exceed 20 characters.");
            RuleFor(p => p.Sku)
                .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.");
            RuleFor(p => p.Barcode)
                .MaximumLength(50).WithMessage("Barcode cannot exceed 50 characters.");
            RuleFor(p => p.CostPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Cost price cannot be negative.");
            RuleFor(p => p.ReorderLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Reorder level cannot be negative.");
            RuleFor(p => p.ProductFeatures)
                .MaximumLength(500).WithMessage("Product features cannot exceed 500 characters.");
            RuleFor(p => p.ProductSpecifications)
                .MaximumLength(500).WithMessage("Product specifications cannot exceed 500 characters.");
            RuleFor(p => p.ProductIngredients)
                .MaximumLength(500).WithMessage("Product ingredients cannot exceed 500 characters.");
            RuleFor(p => p.ProductUsageInstructions)
                .MaximumLength(500).WithMessage("Product usage instructions cannot exceed 500 characters.");
            RuleFor(p => p.ProductWarrantyInformation)
                .MaximumLength(500).WithMessage("Product warranty information cannot exceed 500 characters.");
            RuleFor(p => p.ProductAdditionalInformation)
                .MaximumLength(500).WithMessage("Product additional information cannot exceed 500 characters.");
            RuleFor(p => p.ProductPrice)
                .GreaterThan(0).WithMessage("Product price must be greater than zero.");
            RuleFor(p => p.ProductDescription)
                .NotEmpty().WithMessage("Product description is required.")
                .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");
            RuleFor(p => p.ProductStock)
                .GreaterThanOrEqualTo(0).WithMessage("Product stock cannot be negative.");
        }
    }
}
