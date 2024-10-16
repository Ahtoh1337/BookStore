using BookStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class GenresController : Controller
{
    private ApplicationContext _dbContext;

    public GenresController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var genres = _dbContext.Genres
            .Include(g => g.Books);

        return View(genres);
    }

    [HttpGet()]
    public IActionResult Item(int id)
    {
        var genre = _dbContext.Genres.Include(g => g.Books).FirstOrDefault(g => g.GenreId == id);

        if (genre is not null)
            return View(genre);

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
    public IActionResult Add(string genreName)
    {
        var genre = _dbContext.Genres.FirstOrDefault(g => g.Name == genreName);
        if (genre is null)
        {
            _dbContext.Genres.Add(new Genre() { Name = genreName });
            _dbContext.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance,Marketing")]
    public IActionResult Delete(int id)
    {
        var genre = _dbContext.Genres.Find(id);
        if (genre is not null)
        {
            _dbContext.Genres.Remove(genre);
            _dbContext.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}