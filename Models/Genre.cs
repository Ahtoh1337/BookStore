namespace BookStore.Models;

public class Genre
{
    public int GenreId { get; set; }
    public string? Name { get; set; }

    public IEnumerable<Book> Books { get; set; } = Enumerable.Empty<Book>();
}