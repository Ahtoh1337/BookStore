using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class AuthorsController : Controller
{
    private ApplicationContext _dbContext;

    public AuthorsController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet()]
    public IActionResult Index()
    {
        var authors = _dbContext.Authors.Include(a => a.AuthoredBooks);

        return View(authors);
    }

    [HttpGet("[controller]/[action]/{id}")]
    public IActionResult Item(int id)
    {
        var author = _dbContext.Authors.Include(a => a.AuthoredBooks).FirstOrDefault(a => a.AuthorId == id);

        if (author is not null)
            return View("Author", author);

        return NotFound();
    }
}