namespace BookStore.Models.Api;

public class AuthorApiTarget
{
    public int AuthorId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public int[] AuthoredBooks { get; set; } = [];
}