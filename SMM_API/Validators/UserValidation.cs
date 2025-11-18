using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class UserValidation : AbstractValidator<User>
    {
        public UserValidation()
        {
            RuleFor(u => u.UserEmailAddress)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email address format.")
                .MaximumLength(100).WithMessage("Email address cannot exceed 100 characters.");
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .MaximumLength(100).WithMessage("Password cannot exceed 100 characters.");
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("User name is required.")
                .MaximumLength(50).WithMessage("User name cannot exceed 50 characters.");
            RuleFor(u => u.UserMobileNumber)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^\d{10}$").WithMessage("Mobile number must be exactly 10 digits.")
                .WithMessage("Invalid mobile number format.")
                .MaximumLength(15).WithMessage("Mobile number cannot exceed 15 characters.");
            RuleFor(u => u.UserAddress)
                .NotEmpty().WithMessage("User address is required.")
                .MaximumLength(200).WithMessage("User address cannot exceed 200 characters.");
            RuleFor(u => u.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(u => u.Password).WithMessage("Passwords do not match.");
        }
    }
}
