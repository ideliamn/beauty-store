using BeautyStore.Data;
using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Shared.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;

namespace BeautyStore.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<Category>>> CreateCategoryAsync(List<string> param)
        {
            try
            {
                if (param.Any(string.IsNullOrWhiteSpace))
                {
                    return ApiResponse<List<Category>>.Error(400, "Name cannot be empty.");
                }

                var paramUpper = param.Select(p => p.ToUpper()).ToList();

                var checkCategoryName = await _context.Categories
                    .Where(c => paramUpper.Contains(c.Name.ToUpper()))
                    .Select(c => c.Name)
                    .ToListAsync();

                if (checkCategoryName.Count > 0)
                {
                    var categoryAlreadyExist = string.Join(", ", checkCategoryName);
                    return ApiResponse<List<Category>>.Error(400, $"Category {categoryAlreadyExist} already exist.");
                }

                var newCategories = param.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new Category
                {
                    Name = c
                })
                .ToList();

                _context.Categories.AddRange(newCategories);
                await _context.SaveChangesAsync();

                return ApiResponse<List<Category>>.Success(200, newCategories);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<Category>>.Error(500, "Failed to create category: " + ex.Message);
            }
        }

        public async Task<ApiResponse<Category>> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return ApiResponse<Category>.Error(404, "Category not found.");
            }
            return ApiResponse<Category>.Success(200, category);
        }

        public async Task<ApiResponse<List<Category>>> GetAllCategoryAsync()
        {
            var category = await _context.Categories.ToListAsync();
            if (category.Count == 0)
            {
                return ApiResponse<List<Category>>.Error(400, "No categories found.");
            }
            return ApiResponse<List<Category>>.Success(200, category);
        }

        public async Task<ApiResponse<Category>> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            try
            {
                if (dto.id == null)
                {
                    return ApiResponse<Category>.Error(400, "ID cannot be empty.");
                }
                var category = await _context.Categories.FindAsync(dto.id);

                if (category == null)
                {
                    return ApiResponse<Category>.Error(404, "Category not found.");
                }

                if (!string.IsNullOrWhiteSpace(dto.name))
                {
                    category.Name = dto.name;
                }

                category.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<Category>.Success(200, category);
            }

            catch (Exception ex)
            {
                return ApiResponse<Category>.Error(500, "Failed to update category: " + ex.Message);
            }

        }

        public async Task<ApiResponse<string>> DeleteCategoryAsync(int id)
        {
            try
            {
                if (id == null)
                {
                    return ApiResponse<string>.Error(400, "ID cannot be empty.");
                }

                var category = _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return ApiResponse<string>.Error(404, "Category not found.");
                }

                await _context.Categories.ExecuteDeleteAsync();

                return ApiResponse<string>.Success(200, "Success delete.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(500, "Failed to delete category: " + ex.Message);
            }
        }
    }
}
