using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public class UserBookRating
{
    public int BookId { get; set; }
    public Book? Book { get; set; }

    public required string UserId { get; set; }
    public IdentityUser? User { get; set; }

    public byte Rating { get; set; }
}