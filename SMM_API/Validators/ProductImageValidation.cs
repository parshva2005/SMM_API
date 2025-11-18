using FluentValidation;
using SMM_API.Models;

namespace SMM_API.Validators
{
    public class ProductImageValidation : AbstractValidator<ProductImage>
    {
        public ProductImageValidation()
        {
            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL cannot be empty.")
                .Must(BeAValidUrl).WithMessage("Image URL must be a valid URL.");
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be a non-negative integer.");
        }
        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
