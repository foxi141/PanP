using Microsoft.AspNetCore.Mvc;
using KalkulatorPaliwka.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure PostgreSQL connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // Używamy Npgsql dla PostgreSQL

// Add session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Określenie czasu trwania sesji
    options.Cookie.HttpOnly = true;  // Włączenie bezpieczeństwa ciasteczek sesji
    options.Cookie.IsEssential = true;  // Ustawienie, że ciasteczka są niezbędne
});

// Add any other necessary services, such as logging, etc.
builder.Services.AddLogging(options =>
{
    options.AddConsole();  // Logowanie do konsoli
    options.AddDebug();    // Logowanie do debugera (Visual Studio)
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
app.UseSession(); // Włączenie obsługi sesji

app.UseAuthorization();

// Configure routes and map controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
