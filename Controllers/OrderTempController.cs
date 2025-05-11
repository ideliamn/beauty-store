using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Services;
using BeautyStore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStore.Controllers
{
    [ApiController]
    [Route("api/order_temp")]
    public class OrderTempController : Controller
    {
        private readonly OrderTempService _orderTempService;

        public OrderTempController(OrderTempService orderService)
        {
            _orderTempService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderTempDto orderDto)
        {
            var response = await _orderTempService.CreateOrderTempAsync(orderDto);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<ApiResponse<CreateOrderTempDto>>> GetOrderTempById(string id)
        {
            var response = await _orderTempService.GetOrderTempByIdAsync(id);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<KeyValuePair<string, string>>>>> GetOrderTempAsync(string user_id = "")
        {
            var response = await _orderTempService.GetOrderTempAsync(user_id);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdateOrderDto orderDto)
        {
            var response = await _orderTempService.UpdateOrderTempAsync(orderDto);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(string id)
        {
            var response = await _orderTempService.DeleteOrderTempAsync(id);
            return StatusCode(response.HttpStatus, response);
        }

    }
}
