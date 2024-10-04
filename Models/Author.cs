namespace BookStore.Models;

public class Author
{
    public int AuthorId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Book> AuthoredBooks { get; set; } = new List<Book>();
}