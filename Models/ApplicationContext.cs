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

        builder.Entity<Book>()
            .Property(b => b.Price)
            .HasColumnType("DECIMAL(7, 2)");
    }
}
