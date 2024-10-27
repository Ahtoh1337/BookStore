namespace BookStore.Models.Api;

public class BookApiTarget
{
    public int BookId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int PublishedYear { get; set; }
    public string? Publisher { get; set; }

    public int[] Authors { get; set; } = [];
    public int[] Genres { get; set; } = [];
}