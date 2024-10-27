using BookStore.Models;
using BookStore.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AuthorsController : ControllerBase
{
    private ApplicationContext _dbContext;

    public AuthorsController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult GetAuthors()
    {
        return Ok(_dbContext.Authors
            .Select(a => new AuthorApiTarget() { AuthorId = a.AuthorId, Name = a.Name, Description = a.Description })
            .ToList());
    }


    [HttpGet("{id:int}")]
    public IActionResult GetAuthor(int id)
    {
        return Ok(_dbContext.Authors
            .Select(a => new AuthorApiTarget()
            {
                AuthorId = a.AuthorId,
                Name = a.Name,
                Description = a.Description,
                AuthoredBooks = a.AuthoredBooks.Select(b => b.BookId).ToArray()
            }).FirstOrDefault(a => a.AuthorId == id));
    }


    [HttpPost]
    public IActionResult AddAuthor(AuthorApiTarget target)
    {
        var author = new Author()
        {
            Name = target.Name,
            Description = target.Description,
            AuthoredBooks = _dbContext.Books
                .Where(b => target.AuthoredBooks.Contains(b.BookId)).ToList()
        };
        _dbContext.Authors.Add(author);
        _dbContext.SaveChanges();

        target.AuthorId = author.AuthorId;
        target.AuthoredBooks = author.AuthoredBooks.Select(b => b.BookId).ToArray();
        return Ok(target);
    }


    [HttpPut]
    public IActionResult EditAuthor(AuthorApiTarget target)
    {
        var author = _dbContext.Authors
            .Include(a => a.AuthoredBooks)
            .ThenInclude(b => b.Authors)
            .FirstOrDefault(a => a.AuthorId == target.AuthorId);

        if (author is not null)
        {
            author.Name = target.Name;
            author.Description = target.Description;
            foreach (var book in author.AuthoredBooks.Where(b => !target.AuthoredBooks.Contains(b.BookId)))
            {
                book.Authors.Remove(author);
            }
            author.AuthoredBooks = _dbContext.Books.Where(b => target.AuthoredBooks.Contains(b.BookId)).ToList();
            _dbContext.SaveChanges();
            target.AuthoredBooks = author.AuthoredBooks.Select(b => b.BookId).ToArray();
            return Ok(target);
        }

        return NotFound();
    }


    [HttpDelete("{id:int}")]
    public IActionResult DeleteAuthor(int id)
    {
        var author = _dbContext.Authors.Find(id);

        if (author is not null)
        {
            _dbContext.Authors.Remove(author);
            _dbContext.SaveChanges();
            return Ok(new AuthorApiTarget()
            {
                AuthorId = author.AuthorId,
                Name = author.Name,
                Description = author.Description,
                AuthoredBooks = author.AuthoredBooks.Select(b => b.BookId).ToArray()
            });
        }

        return NotFound();
    }
}