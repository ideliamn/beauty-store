using BeautyStore.DTOs;
using BeautyStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStore.Controllers
{
    [ApiController]
    [Route("api/order")]
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
            var response = await _orderService.CreateOrderAsync(orderDto);
            return StatusCode(response.HttpStatus, response);
        }
    }
}
