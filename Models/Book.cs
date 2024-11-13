using BookStore.Models.Api;

namespace BookStore.Models;

public class Book
{
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
    public string? Publisher { get; set; }

    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<UserBookPurchase> Ratings { get; set; } = new List<UserBookPurchase>();


    public BookApiTarget ToApiTarget()
    {
        return new BookApiTarget()
        {
            BookId = BookId,
            Title = Title,
            Description = Description,
            Price = Price,
            PublishedYear = PublishedYear,
            Publisher = Publisher,
            Authors = Authors.Select(a => a.AuthorId).ToArray(),
            Genres = Genres.Select(g => g.GenreId).ToArray()
        };
    }
}