using Finals.Models;
using Microsoft.EntityFrameworkCore;

namespace Finals.Data
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
    }
}