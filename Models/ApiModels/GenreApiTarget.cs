namespace BookStore.Models.Api;

public class GenreApiTarget
{
    public int GenreId { get; set; }
    public string? Name { get; set; }
    public int[] Books { get; set; } = [];
}