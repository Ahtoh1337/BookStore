using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookStore.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();



var app = builder.Build();

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



/// Seed roles
var rolemgr = app.Services.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
foreach (string role in new string[] { "Admin", "User", "Finance", "Security", "Marketing" })
{
    if (await rolemgr.FindByNameAsync(role) is null)
        await rolemgr.CreateAsync(new IdentityRole() { Name = role });
}

/// Seed admin
var usermgr = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
if (await usermgr.FindByNameAsync("admin") is null)
{
    var admin = new IdentityUser() { UserName = "admin", Email = "admin@mail.com" };
    await usermgr.CreateAsync(admin, "admin12345");
    await usermgr.AddToRoleAsync(admin, "Admin");
}



app.Run();
