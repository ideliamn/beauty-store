using BeautyStore.Data;
using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Shared.Responses;
using BeautyStore.Shared.Checkers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BeautyStore.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CreateOrderDto> _validator;
        private readonly DataExistChecker _checker;
        private readonly OrderTempService _orderTempService;

        public OrderService(ApplicationDbContext context, IValidator<CreateOrderDto> validator, DataExistChecker checker, OrderTempService orderTempService)
        {
            _context = context;
            _validator = validator;
            _checker = checker;
            _orderTempService = orderTempService;
        }

        public async Task<ApiResponse<Orders>> CreateOrderAsync(CreateOrderDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var validationResult = await _validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );
                    return ApiResponse<Orders>.Error(400, "Validation failed.", errors);
                }

                // get data from temp
                var tempData = await _orderTempService.GetOrderTempByIdAsync(dto.order_temp_id);
                Debug.WriteLine("tempdata: " + JsonConvert.SerializeObject(tempData));
                if (tempData.Code == 0)
                {
                    return ApiResponse<Orders>.Error(tempData.HttpStatus, tempData.Message);
                }
                CreateOrderTempDto orderTemp = tempData.Data;
                int userId = orderTemp.user_id;
                List<OrderDetailTempDto> orderTempDetail = tempData.Data.order_detail;

                // check if user exist
                var checkUserExist = await _checker.CheckUserExistAsync(userId);
                if (!checkUserExist)
                {
                    return ApiResponse<Orders>.Error(404, "User not found.");
                }

                // check if product exist
                var (orderDetails, totalPrice, message) = await CreateOrderDetailsAsync(orderTempDetail);
                if (message != null || orderDetails == null)
                {
                    return ApiResponse<Orders>.Error(404, message);
                }

                // insert order
                var order = await InsertOrderAsync(userId, totalPrice);

                // insert details
                await InsertOrderDetailsAsync(order.Id, orderDetails);

                // insert payment
                await InsertPaymentAsync(order.Id, dto.payment_method);

                // update product stock
                foreach (var d in orderTempDetail)
                {
                    await UpdateProductStockAsync(d.product_id, d.quantity);
                }

                // delete temp data
                var deleteTempData = await _orderTempService.DeleteOrderTempAsync(dto.order_temp_id);
                if (deleteTempData.Code == 0)
                {
                    return ApiResponse<Orders>.Error(tempData.HttpStatus, tempData.Message);
                }

                await transaction.CommitAsync();
                return ApiResponse<Orders>.Success(200, order);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<Orders>.Error(500, "Failed to create order: " + ex.Message);
            }
        }

        private async Task<(List<OrderDetail>? orderDetails, int totalPrice, string? message)> CreateOrderDetailsAsync(List<OrderDetailTempDto> orderDetailTemp)
        {
            var orderDetails = new List<OrderDetail>();
            int totalPrice = 0;

            foreach (var o in orderDetailTemp)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == o.product_id);
                if (product == null)
                {
                    return (null, totalPrice, $"Product ID {o.product_id} not found.");
                }

                orderDetails.Add(new OrderDetail
                {
                    ProductId = o.product_id,
                    Quantity = o.quantity
                });

                totalPrice += ((int)product.Price * o.quantity);
            }

            return (orderDetails, totalPrice, null);
        }

        private async Task<Orders> InsertOrderAsync(int userId, int totalPrice)
        {
            var order = new Orders
            {
                UserId = userId,
                TotalAmount = totalPrice
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        private async Task InsertOrderDetailsAsync(int orderId, List<OrderDetail> orderDetails)
        {
            foreach (var o in orderDetails)
            {
                o.OrderId = orderId;
            }

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();
        }

        private async Task InsertPaymentAsync(int orderId, string method)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                PaymentMethod = method,
                PaymentStatus = "COMPLETED",
                PaidAt = DateTime.Now
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateProductStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            product.Stock = product.Stock - quantity;
            product.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

    }
}
