using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.LentByUser)
                .WithMany(u => u.BooksLent)
                .HasForeignKey(b => b.LentByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.CurrentlyBorrowedByUser)
                .WithMany(u => u.BooksBorrowed)
                .HasForeignKey(b => b.CurrentlyBorrowedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
