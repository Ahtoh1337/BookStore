using BookStore.Models;
using BookStore.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private ApplicationContext _dbContext;

    public BooksController(ApplicationContext dbContext)
    {
        _dbContext = dbContext;
    }


    [HttpGet]
    public IActionResult GetBooks()
    {
        return Ok(_dbContext.Books
            .Select(b => new BookApiTarget()
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                Price = b.Price,
                PublishedYear = b.PublishedYear,
                Publisher = b.Publisher
            }).ToList());
    }


    [HttpGet("{id:int}")]
    public IActionResult GetBook(int id)
    {
        return Ok(_dbContext.Books.Select(b => new BookApiTarget()
        {
            BookId = b.BookId,
            Title = b.Title,
            Description = b.Description,
            Price = b.Price,
            PublishedYear = b.PublishedYear,
            Publisher = b.Publisher,
            Authors = b.Authors.Select(a => a.AuthorId).ToArray(),
            Genres = b.Genres.Select(g => g.GenreId).ToArray()
        }).FirstOrDefault(b => b.BookId == id));
    }


    [HttpPost]
    public IActionResult AddBook(BookApiTarget target)
    {
        var book = new Book()
        {
            Title = target.Title,
            Description = target.Description,
            Price = target.Price,
            PublishedYear = target.PublishedYear,
            Publisher = target.Publisher,
            Authors = _dbContext.Authors
                .Where(a => target.Authors.Contains(a.AuthorId)).ToList(),
            Genres = _dbContext.Genres
                .Where(g => target.Genres.Contains(g.GenreId)).ToList()
        };
        _dbContext.Books.Add(book);
        _dbContext.SaveChanges();

        target.BookId = book.BookId;
        target.Authors = book.Authors.Select(a => a.AuthorId).ToArray();
        target.Genres = book.Genres.Select(g => g.GenreId).ToArray();
        return Ok(target);
    }


    [HttpPut]
    public IActionResult EditBook(BookApiTarget target)
    {
        var book = _dbContext.Books
            .Include(b => b.Authors)
            .ThenInclude(a => a.AuthoredBooks)
            .Include(b => b.Genres)
            .ThenInclude(g => g.Books)
            .FirstOrDefault(b => b.BookId == target.BookId);

        if (book is not null)
        {
            book.Title = target.Title;
            book.Description = target.Description;
            book.Price = target.Price;
            book.PublishedYear = target.PublishedYear;
            book.Publisher = target.Publisher;
            foreach (var author in book.Authors.Where(a => !target.Authors.Contains(a.AuthorId)))
            {
                author.AuthoredBooks.Remove(book);
            }
            book.Authors = _dbContext.Authors.Where(a => target.Authors.Contains(a.AuthorId)).ToList();
            foreach (var genre in book.Genres.Where(g => !target.Genres.Contains(g.GenreId)))
            {
                genre.Books.Remove(book);
            }
            book.Genres = _dbContext.Genres.Where(g => target.Genres.Contains(g.GenreId)).ToList();
            _dbContext.SaveChanges();

            target.Authors = book.Authors.Select(a => a.AuthorId).ToArray();
            target.Genres = book.Genres.Select(g => g.GenreId).ToArray();
            return Ok(target);
        }

        return NotFound();
    }


    [HttpDelete("{id:int}")]
    public IActionResult DeleteBook(int id)
    {
        var book = _dbContext.Books.Find(id);

        if (book is not null)
        {
            _dbContext.Remove(book);
            _dbContext.SaveChanges();
            return Ok(new BookApiTarget()
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                PublishedYear = book.PublishedYear,
                Publisher = book.Publisher,
                Authors = book.Authors.Select(a => a.AuthorId).ToArray(),
                Genres = book.Genres.Select(g => g.GenreId).ToArray()
            });
        }

        return NotFound();
    }
}