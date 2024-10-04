using BookStore.Models;
using Microsoft.AspNetCore.Identity;

public class Seed
{
    public async static Task SeedRolesAndAdminAsync(IServiceProvider provider)
    {
        var rolemgr = provider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (string role in new string[] { "Admin", "User", "Finance", "Security", "Marketing" })
        {
            if (await rolemgr.FindByNameAsync(role) is null)
                await rolemgr.CreateAsync(new IdentityRole() { Name = role });
        }

        var usermgr = provider.GetRequiredService<UserManager<IdentityUser>>();
        if (await usermgr.FindByNameAsync("admin") is null)
        {
            var admin = new IdentityUser() { UserName = "admin", Email = "admin@mail.com" };
            await usermgr.CreateAsync(admin, "admin12345");
            await usermgr.AddToRoleAsync(admin, "Admin");
        }
    }



    public static void SeedDbContext(ApplicationContext context)
    {
        if (context.Genres.Count() != 0)
            return;

        var classic = new Genre { Name = "Класика" };
        var poetry = new Genre { Name = "Поезія" };

        var taras = new Author
        {
            Name = "Тарас Шевченко",
            Description = "Український поет, прозаїк, мислитель, живописець, гравер, " +
                    "етнограф, громадський діяч. Національний герой і символ України. Діяч українського " +
                    "національного руху, член Кирило-Мефодіївського братства. Академік Імператорської " +
                    "академії мистецтв (1860). Літературна спадщина Шевченка, центральне місце в якій " +
                    "займає поезія, зокрема збірка «Кобзар», вважається основою сучасної української " +
                    "літератури та значною мірою української літературної мови.",
        };

        var kobzar = new Book
        {
            Title = "Кобзар",
            Publisher = "Просвіта",
            PublishedYear = 2012,
            Price = 170m,
            Description = "Кобзар — так назвав український народ Тараса Григоровича Шевченка. " +
                    "Ім’я цього всесвітньо відомого поета й митця є уособленням незламної сили духу та " +
                    "пророчого дару. Геній Великого Кобзаря світить нам крізь століття. Цілі покоління " +
                    "українців виховувалися на його творах. І сьогодні Шевченкова поезія продовжує " +
                    "надихати нас на нові звершення й перемоги.",
            Genres = [classic, poetry],
            Authors = [taras]
        };

        var gaidamaki = new Book
        {
            Title = "Гайдамаки",
            Publisher = "Фоліо",
            PublishedYear = 2023,
            Price = 150m,
            Description = "«Гайдамаки» — це епічна поема українського поета Тараса Григоровича " +
                    "Шевченка. Поема розповідає про подвиги гайдамаків, українських повстанців, які " +
                    "боролися за свободу свого народу від княжої влади та насильства. Вона пропонує " +
                    "читачеві живе відображення суворих реалій життя народу під час української революції " +
                    "XVIII століття.",
            Genres = [classic],
            Authors = [taras]
        };

        classic.Books = [kobzar, gaidamaki];
        poetry.Books = [kobzar];
        taras.AuthoredBooks = [kobzar, gaidamaki];

        context.Books.AddRange(kobzar, gaidamaki);
        context.SaveChanges();
    }
}