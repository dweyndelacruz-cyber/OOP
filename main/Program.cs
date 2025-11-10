using main.Components;
using main.Data;
using main.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite("Data Source=waterstation.db"));

// Add AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Initialize database - SIMPLE AND DIRECT
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Console.WriteLine("🗃️ CREATING DATABASE AND TABLES...");
        
        // This will create the database AND tables
        bool created = dbContext.Database.EnsureCreated();
        
        if (created)
        {
            Console.WriteLine("✅ DATABASE AND TABLES CREATED SUCCESSFULLY!");
        }
        else
        {
            Console.WriteLine("✅ DATABASE ALREADY EXISTS!");
        }
        
        // Test the database
        try
        {
            int userCount = dbContext.Users.Count();
            Console.WriteLine($"📊 USERS IN DATABASE: {userCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR: {ex.Message}");
            Console.WriteLine("🔄 RECREATING DATABASE...");
            
            // Delete and recreate
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Console.WriteLine("✅ DATABASE RECREATED!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"💥 DATABASE ERROR: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();