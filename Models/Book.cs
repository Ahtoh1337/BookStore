namespace BookStore.Models;

public class Book
{
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public IEnumerable<Author> Authors { get; set; } = Enumerable.Empty<Author>();
    public IEnumerable<Genre> Genres { get; set; } = Enumerable.Empty<Genre>();
    public IEnumerable<UserBookRating> Ratings { get; set; } = Enumerable.Empty<UserBookRating>();
}