using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class BooksController : Controller
{
    private ApplicationContext _dbContext;
    public BooksController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_dbContext.Books.Include(b => b.Authors).Include(b => b.Genres));
    }

    [HttpGet("[controller]/[action]/{id}")]
    public async Task<IActionResult> Item(int id)
    {
        var book = await _dbContext.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book is not null)
            return View("Book", book);

        return NotFound();
    }
}