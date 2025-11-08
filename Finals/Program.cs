using Finals.Components;
using Finals.Data;
using Finals.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// UI services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

// register two separate sqlite DBs and the auth service
builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlite("Data Source=admin.db"));
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseSqlite("Data Source=customer.db"));
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// ensure DBs exist
using (var scope = app.Services.CreateScope())
{
    var adminDb = scope.ServiceProvider.GetRequiredService<AdminDbContext>();
    var customerDb = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
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
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();