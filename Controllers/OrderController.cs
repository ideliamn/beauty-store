using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Services;
using BeautyStore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStore.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            var response = await _orderService.CreateOrderCacheAsync(orderDto);

            return StatusCode(response.HttpStatus, response);
        }

    }
}
