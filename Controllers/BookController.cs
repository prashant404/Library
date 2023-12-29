using Library.Models;
using Library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly LibraryContext _context;

    public BookController(LibraryContext context)
    {
        _context = context;
    }

    // GET: api/Book
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var books = await _context.Books.ToListAsync();
        return Ok(books);
    }

    // GET: api/Book/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(Book book)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        // Set the owner of the book (lender)
        book.LentByUserId = userId;

        _context.Books.Add(book);

        await _context.SaveChangesAsync();

        return CreatedAtAction("GetBook", new { id = book.Id }, book);
    }

    // DELETE: api/Book/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/Book/Borrow/{bookId}
    [HttpPost("Borrow/{bookId}")]
    [Authorize]
    public async Task<ActionResult> BorrowBook(int bookId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var book = await _context.Books.FindAsync(bookId);

        if (book == null)
        {
            return NotFound("Book not found");
        }

        if (!book.IsBookAvailable)
        {
            return BadRequest("Book is not available for borrowing");
        }

        // Update book information
        book.IsBookAvailable = false;
        book.CurrentlyBorrowedByUserId = userId;

        // Decrease TokensAvailable for the user borrowing the book
        user.TokensAvailable -= 1;

        // Increase TokensAvailable for the user lending the book
        var lentByUser = await _context.Users.FindAsync(book.LentByUserId);
        if (lentByUser != null)
        {
            lentByUser.TokensAvailable += 1;
        }

        await _context.SaveChangesAsync();

        return Ok("Book borrowed successfully");
    }


    // GET: api/Book/Search?query={searchQuery}
    [HttpGet("Search")]
    public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string query)
    {
        var books = await _context.Books
            .Where(book =>
                EF.Functions.Like(book.Name, $"%{query}%") ||
                EF.Functions.Like(book.Author, $"%{query}%") ||
                EF.Functions.Like(book.Genre, $"%{query}%"))
            .ToListAsync();

        return Ok(books);
    }

    // GET: api/Book/Details/{bookId}
    [HttpGet("Details/{bookId}")]
    public async Task<ActionResult<Book>> GetBookDetails(int bookId)
    {
        var book = await _context.Books.FindAsync(bookId);

        if (book == null)
        {
            return NotFound("Book not found");
        }

        return Ok(book);
    }

    // GET: api/Book/AllBooksAddedByOtherUsers
    [HttpGet("AllBooksAddedByOtherUsers")]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooksAddedByOtherUsers()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        // Retrieve all books that are not added by the currently logged-in user
        var books = await _context.Books
            .Where(book => book.LentByUserId != userId)
            .ToListAsync();

        return Ok(books);
    }


    private bool BookExists(int id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
