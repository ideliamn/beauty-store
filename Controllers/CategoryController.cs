using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Services;
using BeautyStore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace BeautyStore.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] List<string> param)
        {
            var response = await _categoryService.CreateCategoryAsync(param);

            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Category>>> GetCategoryById(int id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Category>>>> GetAllCategory()
        {
            var response = await _categoryService.GetAllCategoryAsync();
            return StatusCode(response.HttpStatus, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryDto categoryDto)
        {
            var response = await _categoryService.UpdateCategoryAsync(categoryDto);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            return StatusCode(response.HttpStatus, response);
        }
    }
}
