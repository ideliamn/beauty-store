﻿using BeautyStore.DTOs;
using FluentValidation;

namespace BeautyStore.Validators.Users
{
    public class UpdateOrderTempDtoValidator : AbstractValidator<UpdateOrderDto>
    {
        public UpdateOrderTempDtoValidator()
        {
            RuleFor(x => x.id).NotEmpty().WithMessage("ID cannot be empty.");
            RuleFor(x => x.order.user_id).NotEmpty().WithMessage("User ID cannot be empty.");
            RuleFor(x => x.order.order_detail).NotEmpty().WithMessage("Order detail cannot be empty.");
            RuleForEach(x => x.order.order_detail).ChildRules(orderDetail =>
            {
                orderDetail.RuleFor(od => od.product_id)
                    .NotEmpty().WithMessage("Product ID cannot be empty.");

                orderDetail.RuleFor(od => od.quantity)
                    .GreaterThan(0).WithMessage("Quantity must be more than 0.");
            });
        }
    }
}
