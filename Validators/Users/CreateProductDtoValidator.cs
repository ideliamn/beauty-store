using FluentValidation;
using BeautyStore.DTOs;

namespace BeautyStore.Validators.Users
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.brand).NotEmpty().WithMessage("Brand cannot be empty.");
            RuleFor(x => x.name).NotEmpty().WithMessage("Name cannot be empty.");
            RuleFor(x => x.size).NotEmpty().WithMessage("Size cannot be empty.");
            RuleFor(x => x.description).NotEmpty().WithMessage("Description cannot be empty.");
            RuleFor(x => x.price).NotEmpty().WithMessage("Price cannot be empty.");
            RuleFor(x => x.stock).NotEmpty().WithMessage("Stock cannot be empty.");
            RuleFor(x => x.category_id).NotEmpty().WithMessage("Category ID cannot be empty.");
        }
    }
}
