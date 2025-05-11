using BeautyStore.DTOs;
using FluentValidation;

namespace BeautyStore.Validators.Users
{
    public class CreateOrderTempDtoValidator : AbstractValidator<CreateOrderTempDto>
    {
        public CreateOrderTempDtoValidator()
        {
            RuleFor(x => x.user_id).NotEmpty().WithMessage("User ID cannot be empty.");
            RuleFor(x => x.order_detail).NotEmpty().WithMessage("Order detail cannot be empty.");
            RuleForEach(x => x.order_detail).ChildRules(orderDetail =>
            {
                orderDetail.RuleFor(od => od.product_id)
                    .NotEmpty().WithMessage("Product ID cannot be empty.");

                orderDetail.RuleFor(od => od.quantity)
                    .GreaterThan(0).WithMessage("Quantity must be more than 0.");
            });
        }
    }
}
