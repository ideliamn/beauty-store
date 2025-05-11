using BeautyStore.Data;
using BeautyStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautyStore.Shared.Checkers
{
    public class DataExistChecker
    {
        private readonly ApplicationDbContext _context;

        public DataExistChecker(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CheckUserExistAsync(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<Product?> CheckProductValidAsync(int productId, int quantity)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            return product != null && product.Stock >= quantity ? product : null;
        }

    }
}
