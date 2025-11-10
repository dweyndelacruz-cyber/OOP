using main.Data;
using main.Models;
using Microsoft.EntityFrameworkCore;

namespace main.Services
{
    public interface IAuthService
    {
        Task<User?> LoginUserAsync(string username, string password, string role);
        Task<bool> RegisterUserAsync(User user, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginUserAsync(string username, string password, string role)
        {
            try
            {
                Console.WriteLine($"🔐 LOGIN: '{username}' as '{role}'");
                
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.Role == role);

                if (user == null)
                {
                    Console.WriteLine($"❌ USER NOT FOUND");
                    return null;
                }

                // For now, use simple password check - we'll fix hashing later
                bool validPassword = user.PasswordHash == password;
                
                if (validPassword)
                {
                    Console.WriteLine($"✅ LOGIN SUCCESS");
                    return user;
                }
                
                Console.WriteLine($"❌ WRONG PASSWORD");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 LOGIN ERROR: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            try
            {
                Console.WriteLine($"📝 REGISTER: {user.Username}");

                // Check if username exists
                bool exists = await _context.Users.AnyAsync(u => u.Username == user.Username);
                if (exists)
                {
                    Console.WriteLine("❌ USERNAME EXISTS");
                    return false;
                }

                // For now, store plain password - we'll fix hashing later
                user.PasswordHash = password;
                user.CreatedAtUtc = DateTime.UtcNow;
                user.UpdatedAtUtc = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Console.WriteLine("✅ REGISTRATION SUCCESS");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 REGISTRATION ERROR: {ex.Message}");
                return false;
            }
        }
    }
}