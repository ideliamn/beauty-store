using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Services;
using BeautyStore.Shared.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BeautyStore.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            var response = await _productService.CreateProductAsync(productDto);

            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Product>>> GetProductById(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Product>>>> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            return StatusCode(response.HttpStatus, response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto productDto)
        {
            var response = await _productService.UpdateProductAsync(productDto);
            return StatusCode(response.HttpStatus, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            return StatusCode(response.HttpStatus, response);
        }

    }
}
