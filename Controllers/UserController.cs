using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Services;
using BeautyStore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautyStore.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            var response = await _userService.CreateUserAsync(userDto);
            if (response.Code == 0)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            if (response.Code == 0)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync();
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto userDto)
        {
            var response = await _userService.UpdateUserAsync(userDto);
            if (response.Code == 0)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            if (response.Code == 0)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}
