using BeautyStore.Data;
using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Shared.Responses;
using FluentValidation;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BeautyStore.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CreateOrderDto> _validator;
        private readonly IRedisService _redis;

        public OrderService(ApplicationDbContext context, IValidator<CreateOrderDto> validator, IRedisService redis)
        {
            _context = context;
            _validator = validator;
            _redis = redis;
        }

        public async Task<ApiResponse<CreateOrderDto>> CreateOrderCacheAsync(CreateOrderDto dto)
        {
            try
            {
                if (dto == null || dto.order_detail == null || !dto.order_detail.Any())
                {
                    return ApiResponse<CreateOrderDto>.Error(400, "Order invalid.");
                }

                var validationResult = await _validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );
                    return ApiResponse<CreateOrderDto>.Error(400, "Validation failed.", errors);
                }

                string dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string key = $"order:{dto.user_id}:{dateTimeNow}";
                string jsonOrder = JsonConvert.SerializeObject(dto);

                // if not paid in 24 hours, delete from cache
                TimeSpan expirationTime = TimeSpan.FromHours(24);
                await _redis.SetStringAsync(key, jsonOrder, 0, expirationTime);

                return ApiResponse<CreateOrderDto>.Success(200, dto, "Order created, please continue to payment.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CreateOrderDto>.Error(500, "Failed to create order: " + ex.Message);
            }
        }
    }
}
