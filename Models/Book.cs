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
    public ICollection<UserBookRating> Ratings { get; set; } = new List<UserBookRating>();
}