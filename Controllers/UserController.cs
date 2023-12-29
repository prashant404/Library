namespace Library.Controllers
{
    using Library.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly LibraryContext _context;

        public UserController(LibraryContext context)
        {
            _context = context;
        }

        // POST: api/User/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

       
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }



        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> Login(User loginUser)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

            if (user == null)
            {
                return NotFound("Invalid username or password");
            }

            var token = GenerateJwtToken(user);
            user.Token = token;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authentication");
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
     
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(100),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // GET: api/User/Profile
        [HttpGet("Profile")]
        [Authorize]
        public ActionResult<string> GetUsername()
        {
            try
            {
             
                var usernameClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (usernameClaim == null)
                {
                   
                    return BadRequest("Username claim not found");
                }

                var username = usernameClaim.Value;

                
                Console.WriteLine($"Username: {username}");

                return Ok(username);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error in GetUsername: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/User/Tokens
        [HttpGet("Tokens")]
        [Authorize]
        public async Task<ActionResult<int>> GetUserTokens()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.TokensAvailable);
        }


        [HttpGet("UserId")]
        [Authorize]
        public ActionResult<int> GetUserId()
        {
            try
            {
                // Extract user ID from the claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    // Log or return an appropriate response for missing user ID
                    return BadRequest("User ID claim not found");
                }

                var userId = int.Parse(userIdClaim.Value);

                
                Console.WriteLine($"User ID: {userId}");

                return Ok(userId);
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error in GetUserId: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // POST: api/User/BorrowBook/{bookId}
        [HttpPost("BorrowBook/{bookId}")]
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

            if (user.TokensAvailable < 1)
            {
                return BadRequest("Insufficient tokens for borrowing");
            }

            // Update book and user information
            book.IsBookAvailable = false;
            book.CurrentlyBorrowedByUserId = userId;

            user.TokensAvailable -= 1;
            user.BooksBorrowed.Add(book); // Add the entire book object to the list

            await _context.SaveChangesAsync();

            return Ok("Book borrowed successfully");
        }


        // GET: api/User/BorrowedBooks
        [HttpGet("BorrowedBooks/{Id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Book>>> GetBorrowedBooks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var borrowedBooks = await _context.Books
                .Where(book => book.CurrentlyBorrowedByUserId == userId)
                .ToListAsync();

            return Ok(borrowedBooks);
        }

    

        // GET: api/User/LentBooks
        [HttpGet("LentBooks/{Id}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Book>>> GetLentBooks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var lentBooks = await _context.Books
                .Where(book => book.LentByUserId == userId)
                .ToListAsync();

            return Ok(lentBooks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }



    }

}
