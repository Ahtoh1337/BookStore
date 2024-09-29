namespace BookStore.Models;

public class Author
{
    public int AuthorId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public IEnumerable<Book> AuthoredBooks { get; set; } = Enumerable.Empty<Book>();
}