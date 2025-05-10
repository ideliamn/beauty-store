using FluentValidation;
using BeautyStore.DTOs;

namespace BeautyStore.Validators.Users
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.name).NotEmpty().WithMessage("Name cannot be empty.");
            RuleFor(x => x.email).NotEmpty().WithMessage("Email cannot be empty.").EmailAddress().WithMessage("Email is not valid.");
            RuleFor(x => x.password).NotEmpty().WithMessage("Password cannot be empty.").MinimumLength(6).WithMessage("Password must be at least 6 characters.");
            RuleFor(x => x.phone).NotEmpty().WithMessage("Phone cannot be empty.");
            RuleFor(x => x.address).NotEmpty().WithMessage("Address cannot be empty.");
        }
    }
}
