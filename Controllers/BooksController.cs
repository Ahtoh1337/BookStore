using System.Text.Json;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class LegacyBooksController : Controller
{
    private ApplicationContext _dbContext;
    public LegacyBooksController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(_dbContext.Books.Include(b => b.Authors).Include(b => b.Genres));
    }

    [HttpGet]
    public async Task<IActionResult> Item(int id)
    {
        var book = await _dbContext.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.BookId == id);

        if (book is not null)
            return View(book);

        return NotFound();
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance,Marketing")]
    public IActionResult Add()
    {
        ViewBag.Genres = _dbContext.Genres.Select(g => new SelectListItem() { Value = g.GenreId.ToString(), Text = g.Name });
        ViewBag.Authors = _dbContext.Authors.Select(a => new SelectListItem() { Value = a.AuthorId.ToString(), Text = a.Name });
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Finance,Marketing")]
    public IActionResult Add(BookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Genres = _dbContext.Genres.Select(g => new SelectListItem() { Value = g.GenreId.ToString(), Text = g.Name });
            ViewBag.Authors = _dbContext.Authors.Select(a => new SelectListItem() { Value = a.AuthorId.ToString(), Text = a.Name });
            return View();
        }
        
        var book = new Book()
        {
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            PublishedYear = model.PublishedYear,
            Publisher = model.Publisher,
            Genres = _dbContext.Genres.Where(g => model.GenreIds.Contains(g.GenreId)).ToList(),
            Authors = _dbContext.Authors.Where(a => model.AuthorIds.Contains(a.AuthorId)).ToList(),
        };

        _dbContext.Books.Add(book);
        _dbContext.SaveChanges();

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance,Marketing")]
    public IActionResult Delete(int id)
    {
        var book = _dbContext.Books.Find(id);

        if (book is not null)
        {
            _dbContext.Books.Remove(book);
            _dbContext.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }
}