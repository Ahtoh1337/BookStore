using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

var builder = WebApplication.CreateBuilder(args);

// Отримання рядка підключення до бази даних
var connectionString = 
    builder.Configuration
    .GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Реєстрація сервісу контексту застосунку для роботи з базою даних
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Реєстрація та налаштування ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(o =>
{
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.User.RequireUniqueEmail = true;
});

// Реєстрація сервісів для підтримки патерну Model-View-Controller
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();



var app = builder.Build();

// Додавання різних middleware, зокрема для аутентифікації
// та авторизації, підтримки статичних файлів тощо
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapDefaultControllerRoute();

app.MapFallbackToFile("index.html");

// Додавання сутностей до бази даних при першому запуску програми,
// зокрема ролі: Admin, Finance, Security, Marketing, User,
// акаунт адміністратора та декілька книг, авторів та жанрів
await Seed.SeedRolesAndAdminAsync(app.Services.CreateScope().ServiceProvider);
Seed.SeedDbContext(app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>());



app.Run();