using Finals.Data;
using Finals.Models;
using Microsoft.EntityFrameworkCore;

namespace Finals.Services
{
    public class AuthService
    {
        private readonly AdminDbContext _adminDb;
        private readonly CustomerDbContext _customerDb;
        // Keep the currently authenticated user in this scoped service so pages/layouts
        // can check role and perform sign-out. This is an in-memory (scoped) store.
        public User? CurrentUser { get; set; }

        public AuthService(AdminDbContext adminDb, CustomerDbContext customerDb)
        {
            _adminDb = adminDb;
            _customerDb = customerDb;
        }

        public async Task<User?> RegisterUserAsync(string email, string password, string name, bool isAdmin)
        {
            // Check if user already exists in either database
            var adminExists = await _adminDb.Users.AnyAsync(u => u.Email == email);
            var customerExists = await _customerDb.Users.AnyAsync(u => u.Email == email);

            // If user exists in any database, registration should fail
            if (adminExists || customerExists)
            {
                return null;
            }

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
            // First check if the user exists in either database
            var adminUser = await _adminDb.Users.FirstOrDefaultAsync(u => 
                u.Email == email && 
                u.PasswordHash == password);
                
            var customerUser = await _customerDb.Users.FirstOrDefaultAsync(u => 
                u.Email == email && 
                u.PasswordHash == password);

            // Validate based on the attempted login type
            if (isAdmin)
            {
                // Only allow admin login if user exists in admin DB
                if (adminUser != null)
                {
                    CurrentUser = adminUser;
                    CurrentUser.IsAdmin = true;
                    return CurrentUser;
                }
            }
            else
            {
                // Only allow customer login if user exists in customer DB
                if (customerUser != null)
                {
                    CurrentUser = customerUser;
                    CurrentUser.IsAdmin = false;
                    return CurrentUser;
                }
            }

            // Authentication failed
            return null;
        }

        public void SignOut()
        {
            CurrentUser = null;
        }
    }
}
