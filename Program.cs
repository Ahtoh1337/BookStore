using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Отримання рядка підключення до бази даних
var connectionString =
    builder.Configuration
    .GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Реєстрація сервісу контексту застосунку для роботи з базою даних
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlite(connectionString, o =>
    {
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Реєстрація та налаштування ASP.NET Core Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddDefaultUI()
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

app.MapIdentityApi<IdentityUser>();
app.MapRazorPages();
app.MapControllers();
app.MapControllerRoute("default", "{controller}/{action=Index}/{Id?}");

app.MapPost("/logout", async (SignInManager<IdentityUser> signInManager,
    [FromBody] object empty) =>
{
    if (empty != null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.Unauthorized();
})
.RequireAuthorization();

// Додавання сутностей до бази даних при першому запуску програми,
// зокрема ролі: Admin, Finance, Security, Marketing, User,
// акаунт адміністратора та декілька книг, авторів та жанрів
await Seed.SeedRolesAndAdminAsync(app.Services.CreateScope().ServiceProvider);
Seed.SeedDbContext(app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>());



app.Run();