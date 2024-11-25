using System.Text;
using System.Text.RegularExpressions;
using BookStore.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;

public class Seed
{
    public async static Task SeedRolesAndAdminAsync(IServiceProvider provider, IConfiguration config)
    {
        var rolemgr = provider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (string role in new string[] { "Admin", "User", "Finance", "Security", "Marketing" })
        {
            if (await rolemgr.FindByNameAsync(role) is null)
                await rolemgr.CreateAsync(new IdentityRole() { Name = role });
        }

        string adminEmail = config["adminEmail"] ??
            throw new InvalidOperationException("'adminEmail' not found in config or user secrets");
        string adminPassword = config["adminPassword"] ??
            throw new InvalidOperationException("'adminPassword' not found in config or user secrets");

        var usermgr = provider.GetRequiredService<UserManager<IdentityUser>>();
        if (await usermgr.FindByNameAsync(adminEmail) is null)
        {
            var admin = new IdentityUser() { UserName = adminEmail, Email = adminEmail };
            await usermgr.CreateAsync(admin, adminPassword);
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

        _SeedRandomData(context);
    }


    private static void _SeedRandomData(ApplicationContext context)
    {
        Random r = new Random();
        List<Genre> genres = new();
        List<Author> authors = new();
        List<Book> books = new();

        for (int i = 0; i < 10; i++)
        {
            genres.Add(new Genre()
            {
                Name = _RandString(10)
            });
        }

        for (int i = 0; i < 30; i++)
        {
            authors.Add(new Author()
            {
                Name = _RandString(25, 0.2),
                Description = _RandString(300, 0.01),
            });
        }

        for (int i = 0; i < 100; i++)
        {
            var book = new Book()
            {
                Title = _RandString(50, 0.05),
                Description = _RandString(300, 0.01),
                Price = _RandInt(99, 999),
                PublishedYear = _RandInt(1990, 2024),
                Publisher = _RandString(10),
            };
            for (int j = 0; j < 5; j++)
            {
                book.Genres.Add(genres[r.Next() % genres.Count]);
                if (r.NextDouble() < 0.5) break;
            }
            for (int j = 0; j < 4; j++)
            {
                book.Authors.Add(authors[r.Next() % authors.Count]);
                if (r.NextDouble() < 0.8) break;
            }
            books.Add(book);
        }
        context.Genres.AddRange(genres);
        context.Authors.AddRange(authors);
        context.Books.AddRange(books);

        context.SaveChanges();
    }


    private static string _RandString(int maxLength = 10, double cutOff = 0.1)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz_______";
        Random r = new Random();
        StringBuilder str = new StringBuilder();

        str.Append(chars[r.Next() % (chars.Length - 7)]);
        for (int i = 1; i < maxLength; i++)
        {
            str.Append(chars[r.Next() % chars.Length]);
            if (r.NextDouble() < cutOff)
                break;
        }

        string result = str.ToString();
        result = new Regex("_+").Replace(result, " ");
        result = result.TrimEnd();

        return result.ApplyCase(LetterCasing.Sentence);
    }


    private static int _RandInt(int min = 0, int max = 100)
    {
        Random r = new Random();
        return (r.Next() % (max - min + 1)) + min;
    }
}