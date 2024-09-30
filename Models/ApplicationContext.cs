using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models;

public class ApplicationContext : IdentityDbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {

    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<UserBookRating> UserBookRatings { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserBookRating>().HasKey(o => new { o.UserId, o.BookId });

        builder.Entity<Genre>()
            .HasData([
            new Genre { GenreId = 1, Name = "Classic" },
            new Genre { GenreId = 2, Name = "Poetry" },
            new Genre { GenreId = 3, Name = "Science" },
            new Genre { GenreId = 4, Name = "Programming" },
            new Genre { GenreId = 5, Name = "For Kids" }
            ]);

        builder.Entity<Book>()
            .Property(b => b.Price)
            .HasColumnType("DECIMAL(7, 2)");
    }
}
