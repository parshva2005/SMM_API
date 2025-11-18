using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class CustomerValidation : AbstractValidator<Customer>
    {
        public CustomerValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .When(x => !string.IsNullOrEmpty(x.Email))
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.");
            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address cannot exceed 200 characters.");
            RuleFor(x => x.RegistrationDate)
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Registration date cannot be in the future.");
            RuleFor(x => x.IsActive)
                .Must(x => x == true || x == false).WithMessage("IsActive must be a boolean value.");

        }
    }
}
