using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public class UserViewModel
{
    public IdentityUser? User { get; set; }
    public Dictionary<IdentityRole, bool> RelatedRoles { get; set; } = new Dictionary<IdentityRole, bool>();
}