using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers;

[Authorize(Roles = "Admin")]
public class ManageController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly IUserPhoneNumberStore<IdentityUser> _phoneNumberStore;
    private readonly ApplicationContext _dbContext;

    public ManageController(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager,
        IUserStore<IdentityUser> userStore,
        ApplicationContext dbContext
    )
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = (IUserEmailStore<IdentityUser>)_userStore;
        _phoneNumberStore = (IUserPhoneNumberStore<IdentityUser>)_userStore;
        _dbContext = dbContext;
    }


    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public IActionResult Roles()
    {
        return View(_roleManager.Roles);
    }


    [HttpGet("[controller]/[action]/{id}")]
    public async Task<IActionResult> Roles(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role is not null)
            return View("Role", new RoleViewModel() { Role = role, RelatedUsers = await _userManager.GetUsersInRoleAsync(role.Name ?? "") });

        return NotFound();
    }


    [HttpGet]
    public IActionResult Users()
    {
        return View(_userManager.Users);
    }


    [HttpGet("[controller]/[action]/{id}")]
    public async Task<IActionResult> Users(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is not null)
        {
            var roles = _roleManager.Roles.ToDictionary(r => r, elementSelector: r =>
            {
                return _dbContext.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id && ur.RoleId == r.Id) is not null;
            });

            return View("User", new UserViewModel() { User = user, RelatedRoles = roles });
        }

        return NotFound();
    }


    [HttpPost]
    public async Task<IActionResult> Users(UserViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model?.User?.Id ?? "");

        if (user is not null)
        {
            await _userStore.SetUserNameAsync(user, model?.User?.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, model?.User?.Email, CancellationToken.None);
            await _phoneNumberStore.SetPhoneNumberAsync(user, model?.User?.PhoneNumber, CancellationToken.None);
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Users), new { id = user.Id });
        }

        return NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> ToggleRole(string userId, string roleId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        var role = (await _roleManager.FindByIdAsync(roleId))?.Name;

        if (user is not null && role is not null)
        {
            if ((await _userManager.GetUserAsync(User))?.Id == user.Id && role == "Admin")
                return RedirectToAction(nameof(Users), routeValues: new { Id = userId });

            if (await _userManager.IsInRoleAsync(user, role))
                await _userManager.RemoveFromRoleAsync(user, role);
            else
                await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction(nameof(Users), routeValues: new { Id = userId });
    }


    [HttpGet]
    public async Task<ActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is not null && (await _userManager.GetUserAsync(User))?.Id != user.Id)
        {
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Users));
        }

        return RedirectToAction(nameof(Users), new { Id = id });
    }
}