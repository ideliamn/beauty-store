using BeautyStore.Data;
using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Shared.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BeautyStore.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CreateOrderDto> _validatorCreate;
        private readonly IValidator<UpdateOrderDto> _validatorUpdate;
        private readonly IRedisService _redis;

        public OrderService(ApplicationDbContext context, IValidator<CreateOrderDto> validatorCreate, IValidator<UpdateOrderDto> validatorUpdate, IRedisService redis)
        {
            _context = context;
            _redis = redis;
            _validatorCreate = validatorCreate;
            _validatorUpdate = validatorUpdate;
        }

        public async Task<ApiResponse<CreateOrderDto>> CreateOrderTempAsync(CreateOrderDto dto)
        {
            try
            {
                if (dto == null || dto.order_detail == null || !dto.order_detail.Any())
                {
                    return ApiResponse<CreateOrderDto>.Error(400, "Order invalid.");
                }

                var validationResult = await _validatorCreate.ValidateAsync(dto);
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

        public async Task<ApiResponse<CreateOrderDto>> GetOrderTempByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ApiResponse<CreateOrderDto>.Error(400, "ID cannot be empty.");
            }

            var orderTemp = await _redis.GetStringAsync(id, 0);
            if (string.IsNullOrEmpty(orderTemp))
            {
                return ApiResponse<CreateOrderDto>.Error(404, "Order not found.");
            }

            var dto = JsonConvert.DeserializeObject<CreateOrderDto>(orderTemp);
            if (dto == null)
            {
                return ApiResponse<CreateOrderDto>.Error(500, "Failed to get order data.");
            }

            return ApiResponse<CreateOrderDto>.Success(200, dto);
        }

        public async Task<ApiResponse<List<OrderTempDto>>> GetOrderTempAsync(string user_id = "")
        {
            try
            {
                var keyValuePairs = await _redis.GetKeyValueAsync(0, user_id);

                if (keyValuePairs == null || keyValuePairs.Count == 0)
                {
                    return ApiResponse<List<OrderTempDto>>.Error(404, "Order not found.");
                }

                var result = new List<OrderTempDto>();

                foreach (var kv in keyValuePairs)
                {
                    var value = kv.Value;
                    var dto = JsonConvert.DeserializeObject<CreateOrderDto>(value);

                    if (dto != null)
                    {
                        result.Add(new OrderTempDto
                        {
                            Key = kv.Key,
                            Value = dto
                        });
                    }
                }

                return ApiResponse<List<OrderTempDto>>.Success(200, result);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<OrderTempDto>>.Error(500, "Failed to get order data: " + ex.Message);
            }
        }

        public async Task<ApiResponse<UpdateOrderDto>> UpdateOrderTempAsync(UpdateOrderDto dto)
        {
            try
            {
                if (dto == null || dto.order == null)
                {
                    return ApiResponse<UpdateOrderDto>.Error(400, "Order invalid.");
                }

                var validationResult = await _validatorUpdate.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );
                    return ApiResponse<UpdateOrderDto>.Error(400, "Validation failed.", errors);
                }

                bool checkKey = await _redis.CheckKeyAsync(dto.key);
                if (!checkKey)
                {
                    return ApiResponse<UpdateOrderDto>.Error(404, "Order not found.");
                }

                string dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string key = dto.key;
                string jsonOrder = JsonConvert.SerializeObject(dto.order);

                // if not paid in 24 hours, delete from cache
                TimeSpan expirationTime = TimeSpan.FromHours(24);
                await _redis.SetStringAsync(key, jsonOrder, 0, expirationTime);

                return ApiResponse<UpdateOrderDto>.Success(200, dto, "Order updated, please continue to payment.");
            }
            catch (Exception ex)
            {
                return ApiResponse<UpdateOrderDto>.Error(500, "Failed to update order: " + ex.Message);
            }
        }

        public async Task<ApiResponse<string>> DeleteOrderTempAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ApiResponse<string>.Error(400, "ID cannot be empty.");
            }

            bool checkKey = await _redis.CheckKeyAsync(id);
            if (!checkKey)
            {
                return ApiResponse<string>.Error(404, "Order not found.");
            }

            bool delete = await _redis.DeleteByKeyAsync(id);
            if (!delete) {
                return ApiResponse<string>.Error(500, "Failed to delete order.");
            }

            return ApiResponse<string>.Success(200, "Order deleted.");
        }
    }
}
