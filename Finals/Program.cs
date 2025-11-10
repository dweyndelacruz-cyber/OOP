using Finals.Components;
using Finals.Data;
using Finals.Services;
using Microsoft.AspNetCore.Components.Authorization; // Needed for AuthenticationStateProvider
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// UI services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

// ----------------------------------------------------------------------
// AUTHENTICATION AND AUTHORIZATION SERVICES
// ----------------------------------------------------------------------

// 1. Add Blazor's core Authorization system
builder.Services.AddAuthorization(); 

// 2. Register the custom provider to handle the user's login state and roles
builder.Services.AddScoped<AuthenticationStateProvider, Finals.Services.CustomAuthStateProvider>();

// 3. Register your AuthService
builder.Services.AddScoped<AuthService>(); 

// ----------------------------------------------------------------------
// DATABASE SERVICES
// ----------------------------------------------------------------------

// Register two separate SQLite DBs
builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AdminConnection") ?? "Data Source=admin.db"));

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CustomerConnection") ?? "Data Source=customer.db"));


var app = builder.Build();

// Ensure DBs exist on startup
using (var scope = app.Services.CreateScope())
{
    var adminDb = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
    var customerDb = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    // This will create the database file if it doesn't exist
    adminDb.Database.EnsureCreated(); 
    customerDb.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CRITICAL: Must be called before MapBlazorHub
app.UseAuthorization(); 

app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();