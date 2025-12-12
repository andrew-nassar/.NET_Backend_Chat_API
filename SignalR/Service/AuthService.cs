using Microsoft.EntityFrameworkCore;
using SignalR.Dbcontext;
using SignalR.Models;

namespace SignalR.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(RegisterDto dto)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (existing != null) return null; // بدل Exception //throw new Exception("Phone already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                ProfilePic = "",
                IsOnline = false,
                LastSeen = DateTime.UtcNow
            };

            // Hash password (simple example, replace with secure hashing)
            user.PasswordHash = dto.Password;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (user == null || user.PasswordHash != dto.Password)
                return null; // بدل throw

            return user;
        }
    }
}
