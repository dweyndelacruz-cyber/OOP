using Finals.Data;
using Finals.Models;
using Microsoft.EntityFrameworkCore;

namespace Finals.Services
{
    public class AuthService
    {
        private readonly AdminDbContext _adminDb;
        private readonly CustomerDbContext _customerDb;

        public AuthService(AdminDbContext adminDb, CustomerDbContext customerDb)
        {
            _adminDb = adminDb;
            _customerDb = customerDb;
        }

        public async Task<User> RegisterUserAsync(string email, string password, string name, bool isAdmin)
        {
            var user = new User
            {
                Email = email,
                Name = name,
                PasswordHash = password, // replace with real hashing before production
                IsAdmin = isAdmin
            };

            if (isAdmin)
            {
                _adminDb.Users.Add(user);
                await _adminDb.SaveChangesAsync();
            }
            else
            {
                _customerDb.Users.Add(user);
                await _customerDb.SaveChangesAsync();
            }

            return user;
        }

        public async Task<User?> AuthenticateAsync(string email, string password, bool isAdmin)
        {
            if (isAdmin)
                return await _adminDb.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
            return await _customerDb.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        }
    }
}
