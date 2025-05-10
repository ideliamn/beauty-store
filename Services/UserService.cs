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
            var users = await _context.Users.AsNoTracking().ToListAsync();
            if (users.Count == 0)
            {
                return ApiResponse<List<User>>.Error("No users found.");
            }
            return ApiResponse<List<User>>.Success(users);
        }

        public async Task<ApiResponse<User>> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return ApiResponse<User>.Error("User not found.");
            }
            return ApiResponse<User>.Success(user);
        }

        public async Task<ApiResponse<User>> CreateUserAsync(CreateUserDto userDto)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.password);

            var user = new User
            {
                Name = userDto.name,
                Email = userDto.email,
                Password = passwordHash,
                Phone = userDto.phone,
                Address = userDto.address
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ApiResponse<User>.Success(user);
        }
    }

}
