using Microsoft.AspNetCore.Mvc;
using KalkulatorPaliwka.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure PostgreSQL connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // Use Npgsql for PostgreSQL

// Add session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;  // Enable HTTP-only cookies for session
    options.Cookie.IsEssential = true;  // Mark session cookies as essential
});

// Add any other necessary services, such as logging, etc.
builder.Services.AddLogging(options =>
{
    options.AddConsole();  // Log to console
    options.AddDebug();    // Log to debugger (e.g., Visual Studio)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session middleware
app.UseSession();

app.UseAuthorization();

// Configure routes and map controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Add routing for AdminDashboardController
app.MapControllerRoute(
    name: "admin_dashboard",
    pattern: "Admin/{action=Index}/{id?}",
    defaults: new { controller = "AdminDashboard" });

// Other specific routes
app.MapControllerRoute(
    name: "admin_user_history",
    pattern: "Admin/UserHistory",
    defaults: new { controller = "Admin", action = "UserHistory" });

app.MapControllerRoute(
    name: "admin_assign_vehicle",
    pattern: "Admin/AssignVehiclePost",
    defaults: new { controller = "Admin", action = "AssignVehiclePost" });

// Default route for Account/Login
app.MapControllerRoute(
    name: "login",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
