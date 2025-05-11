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
        private readonly OrderTempService _orderService;

        public OrderController(OrderTempService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            var response = await _orderService.CreateOrderTempAsync(orderDto);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<ApiResponse<CreateOrderDto>>> GetOrderTempById(string id)
        {
            var response = await _orderService.GetOrderTempByIdAsync(id);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<KeyValuePair<string, string>>>>> GetOrderTempAsync(string user_id = "")
        {
            var response = await _orderService.GetOrderTempAsync(user_id);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderDto orderDto)
        {
            var response = await _orderService.UpdateOrderTempAsync(orderDto);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var response = await _orderService.DeleteOrderTempAsync(id);
            return StatusCode(response.HttpStatus, response);
        }

    }
}
