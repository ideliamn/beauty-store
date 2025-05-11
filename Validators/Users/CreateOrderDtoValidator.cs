using BeautyStore.DTOs;
using FluentValidation;

namespace BeautyStore.Validators.Users
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.order_temp_id).NotEmpty().WithMessage("Order Temp ID cannot be empty.");
            RuleFor(x => x.payment_method).NotEmpty().WithMessage("Payment method cannot be empty.");
        }
    }
}
