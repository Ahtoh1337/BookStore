using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookStore.Pages;

[Authorize(Roles = "Finance,Security")]
public class StaticModel : PageModel
{
    
}