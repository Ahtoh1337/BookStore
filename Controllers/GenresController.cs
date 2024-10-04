using BookStore.Models;
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
            .Include(g => g.Books)
            .ToDictionary(keySelector: g => g, elementSelector: g => g.Books.Count);

        return View(genres);
    }

    [HttpGet("[controller]/[action]/{id}")]
    public IActionResult Item(int id)
    {
        var genre = _dbContext.Genres.Include(g => g.Books).FirstOrDefault(g => g.GenreId == id);

        if (genre is not null)
            return View("Genre", genre);

        return NotFound();
    }
}