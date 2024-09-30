using Microsoft.AspNetCore.Identity;

namespace BookStore.Models;

public class RoleViewModel
{
    public IdentityRole? Role { get; set; }
    public IEnumerable<IdentityUser> RelatedUsers { get; set; } = Enumerable.Empty<IdentityUser>();
}