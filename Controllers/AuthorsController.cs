using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class LegacyAuthorsController : Controller
{
    private ApplicationContext _dbContext;

    public LegacyAuthorsController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet()]
    public IActionResult Index()
    {
        var authors = _dbContext.Authors.Include(a => a.AuthoredBooks);

        return View(authors);
    }

    [HttpGet]
    public IActionResult Item(int id)
    {
        var author = _dbContext.Authors.Include(a => a.AuthoredBooks).FirstOrDefault(a => a.AuthorId == id);

        if (author is not null)
            return View(author);

        return NotFound();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance,Marketing")]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Finance,Marketing")]
    public IActionResult Add(AuthorViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var author = new Author() { Name = model.Name, Description = model.Description };

        _dbContext.Authors.Add(author);
        _dbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance,Marketing")]
    public IActionResult Delete(int id)
    {
        var author = _dbContext.Authors.Find(id);

        if (author is not null)
        {
            _dbContext.Authors.Remove(author);
            _dbContext.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }
}