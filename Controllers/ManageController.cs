using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

[Authorize(Roles = "Admin")]
public class ManageController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public ManageController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    [HttpGet]
    public IActionResult Roles()
    {
        return View(_roleManager.Roles.Select(r => r.Name));
    }
}