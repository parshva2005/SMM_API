using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class RoleValidation : AbstractValidator<Role>
    {
        public RoleValidation()
        {
            RuleFor(r => r.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");
            RuleFor(r => r.CreationDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Creation date cannot be in the future.");
            RuleFor(r => r.IsRemove)
                .Must(value => value == true || value == false).WithMessage("IsRemove must be a boolean value.");
        }
    }
}
