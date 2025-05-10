using BeautyStore.Data;
using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Shared.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BeautyStore.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CreateProductDto> _validator;
        public ProductService(ApplicationDbContext context, IValidator<CreateProductDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ApiResponse<Product>> CreateProductAsync(CreateProductDto dto)
        {
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
                    return ApiResponse<Product>.Error(400, "Validation failed.", errors);
                }

                var checkCategoryExist = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.category_id);
                if (checkCategoryExist != null)
                {
                    return ApiResponse<Product>.Error(404, "Category not found.");
                }

                var product = new Product
                {
                    Brand = dto.brand,
                    Name = dto.name,
                    Size = dto.size,
                    Description = dto.description,
                    Price = dto.price,
                    Stock = dto.stock,
                    CategoryId = dto.category_id
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return ApiResponse<Product>.Success(200, product);

            }
            catch (Exception ex)
            {
                return ApiResponse<Product>.Error(500, "Failed to create product: " + ex.Message);
            }
        }

        public async Task<ApiResponse<Product>> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (product == null)
            {
                return ApiResponse<Product>.Error(404, "Product not found.");
            }
            return ApiResponse<Product>.Success(200, product);
        }

        public async Task<ApiResponse<List<Product>>> GetAllProductsAsync()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();
            if (products.Count == 0)
            {
                return ApiResponse<List<Product>>.Error(400, "No products found.");
            }
            return ApiResponse<List<Product>>.Success(200, products);
        }

        public async Task<ApiResponse<Product>> UpdateProductAsync(UpdateProductDto dto)
        {
            try
            {
                if (dto.id == null)
                {
                    return ApiResponse<Product>.Error(400, "ID cannot be empty.");
                }
                var product = await _context.Products.FindAsync(dto.id);

                if (product == null)
                {
                    return ApiResponse<Product>.Error(404, "Product not found.");
                }

                if (!string.IsNullOrWhiteSpace(dto.brand))
                {
                    product.Brand = dto.brand;
                }

                if (!string.IsNullOrWhiteSpace(dto.name))
                {
                    product.Name = dto.name;
                }

                if (!string.IsNullOrWhiteSpace(dto.size))
                {
                    product.Size = dto.size;
                }

                if (!string.IsNullOrWhiteSpace(dto.description))
                {
                    product.Description = dto.description;
                }

                if (dto.price.HasValue)
                {
                    product.Price = (int)dto.price;
                }

                if (dto.stock.HasValue)
                {
                    product.Stock = (int)dto.stock;
                }

                if (dto.category_id.HasValue)
                {
                    var checkCategoryExist = await _context.Categories.FirstOrDefaultAsync(c => c.Id == dto.category_id);

                    if (checkCategoryExist != null)
                    {
                        return ApiResponse<Product>.Error(404, "Category not found.");
                    }

                    product.CategoryId = (int)dto.category_id;
                }

                product.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<Product>.Success(200, product);
            }

            catch (Exception ex)
            {
                return ApiResponse<Product>.Error(500, "Failed to update product: " + ex.Message);
            }

        }

        public async Task<ApiResponse<string>> DeleteProductAsync(int id)
        {
            try
            {
                if (id == null)
                {
                    return ApiResponse<string>.Error(400, "ID cannot be empty.");
                }

                var product = _context.Products.FindAsync(id);
                if (product == null)
                {
                    return ApiResponse<string>.Error(404, "Product not found.");
                }

                await _context.Products.ExecuteDeleteAsync();

                return ApiResponse<string>.Success(200, "Success delete.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(500, "Failed to delete product: " + ex.Message);
            }
        }
    }
}
