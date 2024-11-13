using BookStore.Models;
using BookStore.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
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
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .Select(b => b.ToApiTarget())
            .ToList());
    }


    [HttpGet("{id:int}")]
    public IActionResult GetBook(int id)
    {
        return Ok(_dbContext.Books
            .Select(b => new BookApiTarget()
            {
                BookId = b.BookId,
                Title = b.Title,
                Description = b.Description,
                Price = b.Price,
                PublishedYear = b.PublishedYear,
                Publisher = b.Publisher,
                Authors = b.Authors.Select(a => a.AuthorId).ToArray(),
                Genres = b.Genres.Select(g => g.GenreId).ToArray()
            })
            .FirstOrDefault(b => b.BookId == id));
    }


    [Authorize(Roles = "Admin,Finance,Marketing")]
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


    [Authorize(Roles = "Admin,Finance,Marketing")]
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


    [Authorize(Roles = "Admin,Finance,Marketing")]
    [HttpDelete("{id:int}")]
    public IActionResult DeleteBook(int id)
    {
        var book = _dbContext.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefault(b => b.BookId == id);

        if (book is not null)
        {
            _dbContext.Remove(book);
            _dbContext.SaveChanges();
            return Ok(book.ToApiTarget());
        }

        return NotFound();
    }


    [Authorize]
    [HttpGet("purchase/{email}")]
    public IActionResult GetPurchasedBooks(string email)
    {
        if (_dbContext.Users.FirstOrDefault(u => u.Email == email) is null)
            return NotFound();


        bool predicate(UserBookPurchase p) => p.User?.Email == email;
        PurchaseApiTarget? convert(UserBookPurchase p)
        {
            if (p.User is null)
                return null;

            return new PurchaseApiTarget()
            {
                UserEmail = p.User.Email,
                BookId = p.BookId,
                Rating = p.Rating
            };
        }

        var books = _dbContext.UserBookPurchases
            .Include(p => p.User)
            .Include(p => p.Book)
            .Where(predicate)
            .Select(convert)
            .ToList();

        return Ok(books);
    }


    [Authorize]
    [HttpGet("purchase/{email}/{bookId:int}")]
    public IActionResult GetPurchasedBook(string email, int bookId)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

        if (user is null)
            return NotFound();

        var purchase = _dbContext.UserBookPurchases
            .FirstOrDefault(p => p.UserId == user.Id && p.BookId == bookId);

        if (purchase is null)
            return NotFound();

        return Ok(new PurchaseApiTarget()
        {
            UserEmail = email,
            BookId = bookId,
            Rating = purchase.Rating
        });
    }


    [Authorize]
    [HttpPost("purchase")]
    public IActionResult PurchaseBook(PurchaseApiTarget target)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == target.UserEmail);
        var book = _dbContext.Books.Find(target.BookId);

        if (user is null)
            return NotFound($"User {target.UserEmail} not found");

        if (book is null)
            return NotFound("Book not found");

        var purchase = _dbContext.UserBookPurchases
            .FirstOrDefault(p => p.UserId == user.Id && p.BookId == book.BookId);

        if (purchase is not null)
            return BadRequest();

        purchase = new UserBookPurchase()
        {
            UserId = user.Id,
            BookId = book.BookId
        };
        _dbContext.UserBookPurchases.Add(purchase);
        _dbContext.SaveChanges();

        return Ok(target);
    }


    [Authorize]
    [HttpPut("purchase")]
    public IActionResult UpdatePurchasedBook(PurchaseApiTarget target)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == target.UserEmail);

        if (user is null)
            return NotFound();

        var purchase = _dbContext.UserBookPurchases
            .FirstOrDefault(p => p.UserId == user.Id && p.BookId == target.BookId);

        if (purchase is null)
            return NotFound();

        purchase.Rating = target.Rating;
        _dbContext.SaveChanges();

        return Ok(target);
    }


    [Authorize]
    [HttpDelete("purchase/{email}/{bookId:int}")]
    public IActionResult DeletePurchasedBook(string email, int bookId)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

        if (user is null)
            return NotFound();

        var purchase = _dbContext.UserBookPurchases
            .FirstOrDefault(p => p.UserId == user.Id && p.BookId == bookId);

        if (purchase is null)
            return NotFound();

        _dbContext.UserBookPurchases.Remove(purchase);
        _dbContext.SaveChanges();

        return Ok(new PurchaseApiTarget()
        {
            UserEmail = email,
            BookId = bookId,
            Rating = purchase.Rating
        });
    }
}