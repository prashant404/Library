using System.Text.Json.Serialization;

namespace Library.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Rating { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public bool IsBookAvailable { get; set; }
        public string Description { get; set; }

        // Foreign Key to User for the user who lent the book
        public int? LentByUserId { get; set; }

        [JsonIgnore]
        public User LentByUser { get; set; }
        public int? CurrentlyBorrowedByUserId { get; set; }

        [JsonIgnore]
        public User CurrentlyBorrowedByUser { get; set; }
    }
}
