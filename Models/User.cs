using System.Collections.Generic;

namespace Library.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int TokensAvailable { get; set; }
        public string Token { get; set; }


        public List<Book> BooksBorrowed { get; set; }

  
        public List<Book> BooksLent { get; set; }
    }


}
