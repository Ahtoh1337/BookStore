namespace BookStore.Models.Api;

public class PurchaseApiTarget
{
    public string? UserEmail { get; set; }

    public int BookId { get; set; }

    public int? Rating { get; set; }
}