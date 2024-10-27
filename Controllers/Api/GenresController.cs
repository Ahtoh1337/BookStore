using BookStore.Models;
using BookStore.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GenresController : ControllerBase
{
    private ApplicationContext _dbContext;

    public GenresController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }


    [HttpGet]
    public IActionResult GetGenres()
    {
        return Ok(_dbContext.Genres.Select(g => new GenreApiTarget() { Name = g.Name, GenreId = g.GenreId }).ToList());
    }


    [HttpGet("{id:int}")]
    public IActionResult GetGenre(int id)
    {
        return Ok(_dbContext.Genres
        .Include(g => g.Books)
        .AsSplitQuery()
        .Select(g => new GenreApiTarget() { GenreId = g.GenreId, Name = g.Name, Books = g.Books.Select(b => b.BookId).ToArray() })
        .FirstOrDefault(g => g.GenreId == id));
    }


    [HttpPost]
    public IActionResult AddGenre(GenreApiTarget target)
    {
        var genre = new Genre()
        {
            Name = target.Name,
            Books = _dbContext.Books.Where(b => target.Books.Contains(b.BookId)).ToList()
        };
        _dbContext.Genres.Add(genre);
        _dbContext.SaveChanges();
        
        target.GenreId = genre.GenreId;
        target.Books = genre.Books.Select(b => b.BookId).ToArray();
        return Ok(target);
    }


    [HttpPut]
    public IActionResult EditGenre(GenreApiTarget target)
    {
        var genre = _dbContext.Genres
            .Include(g => g.Books)
            .ThenInclude(b => b.Genres)
            .AsSplitQuery()
            .FirstOrDefault(g => g.GenreId == target.GenreId);

        if (genre is not null)
        {
            genre.Name = target.Name;
            foreach (var book in genre.Books.Where(b => !target.Books.Contains(b.BookId)))
            {
                book.Genres.Remove(genre);
            }
            genre.Books = _dbContext.Books.Where(b => target.Books.Contains(b.BookId)).ToList();
            _dbContext.SaveChanges();

            target.Books = genre.Books.Select(b => b.BookId).ToArray();
            return Ok(target);
        }

        return NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteGenre(int id)
    {
        var genre = _dbContext.Genres.Find(id);

        if (genre is not null)
        {
            _dbContext.Remove(genre);
            _dbContext.SaveChanges();
            return Ok(new GenreApiTarget()
            {
                GenreId = genre.GenreId,
                Name = genre.Name,
                Books = genre.Books.Select(b => b.BookId).ToArray()
            });
        }

        return NotFound();
    }
}