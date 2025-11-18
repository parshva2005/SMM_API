using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class CategoryValidation : AbstractValidator<Category>
    {
        public CategoryValidation()
        {
            RuleFor(ct => ct.CategoryName)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(50).WithMessage("Category name cannot exceed 50 characters.");
            RuleFor(ct => ct.IsRemoved)
                .Must(value => value == true || value == false).WithMessage("IsRemove must be a boolean value.");
        }
    }
}
