using BeautyStore.Data;
using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Shared.Responses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeautyStore.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<CreateUserDto> _validator;

        public UserService(ApplicationDbContext context, IValidator<CreateUserDto> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<ApiResponse<User>> CreateUserAsync(CreateUserDto dto)
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
                    return ApiResponse<User>.Error(400, "Validation failed.", errors);
                }

                var checkEmailExist = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.email);
                if (checkEmailExist != null)
                {
                    return ApiResponse<User>.Error(400, "Email is already registered.");
                }

                var checkPhoneExist = await _context.Users.FirstOrDefaultAsync(u => u.Phone == dto.phone);
                if (checkPhoneExist != null)
                {
                    return ApiResponse<User>.Error(400, "Phone is already registered.");
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.password);

                var user = new User
                {
                    Name = dto.name,
                    Email = dto.email,
                    Password = passwordHash,
                    Phone = dto.phone,
                    Address = dto.address
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return ApiResponse<User>.Success(200, user);

            }
            catch (Exception ex)
            {
                return ApiResponse<User>.Error(500, "Failed to create user: " + ex.Message);
            }
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return ApiResponse<User>.Error(404, "User not found.");
            }
            return ApiResponse<User>.Success(200, user);
        }

        public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            if (users.Count == 0)
            {
                return ApiResponse<List<User>>.Error(400, "No users found.");
            }
            return ApiResponse<List<User>>.Success(200, users);
        }

        public async Task<ApiResponse<User>> UpdateUserAsync(UpdateUserDto dto)
        {
            try
            {
                if (dto.id == null)
                {
                    return ApiResponse<User>.Error(400, "ID cannot be empty.");
                }
                var user = await _context.Users.FindAsync(dto.id);

                if (user == null)
                {
                    return ApiResponse<User>.Error(404, "User not found.");
                }

                if (!string.IsNullOrWhiteSpace(dto.name))
                {
                    user.Name = dto.name;
                }

                if (!string.IsNullOrWhiteSpace(dto.email))
                {
                    user.Email = dto.email;
                }

                if (!string.IsNullOrWhiteSpace(dto.password))
                {
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.password);
                    user.Password = passwordHash;
                }

                if (!string.IsNullOrWhiteSpace(dto.phone))
                {
                    user.Phone = dto.phone;
                }

                if (!string.IsNullOrWhiteSpace(dto.address))
                {
                    user.Address = dto.address;
                }

                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<User>.Success(200, user);
            }

            catch (Exception ex) 
            {
                return ApiResponse<User>.Error(500, "Failed to update user: " + ex.Message);
            }

        }

        public async Task<ApiResponse<string>> DeleteUserAsync(int id)
        {
            try
            {
                if (id == null)
                {
                    return ApiResponse<string>.Error(400, "ID cannot be empty.");
                }

                var user = _context.Users.FindAsync(id);
                if (user == null)
                {
                    return ApiResponse<string>.Error(404, "User not found.");
                }

                await _context.Users.ExecuteDeleteAsync();

                return ApiResponse<string>.Success(200, "Success delete.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(500, "Failed to delete user: " + ex.Message);
            }
        }
    }

}
