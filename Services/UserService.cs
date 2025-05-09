using BeautyStore.Data;
using BeautyStore.DTOs;
using BeautyStore.Models;
using BeautyStore.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace BeautyStore.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            if (users.Count == 0)
            {
                return ApiResponse<List<User>>.Error("No users found.");
            }
            return ApiResponse<List<User>>.Success(users);
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return ApiResponse<User>.Error("User not found.");
            }
            return ApiResponse<User>.Success(user);
        }

        public async Task<ApiResponse<User>> CreateUserAsync(CreateUserDto userDto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ApiResponse<User>.Success(user);
        }
    }

}
