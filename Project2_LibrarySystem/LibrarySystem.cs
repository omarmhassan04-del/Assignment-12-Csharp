using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public ICollection<Book> Books { get; set; } = new HashSet<Book>();
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public ICollection<Loan> Loans { get; set; } = new HashSet<Loan>();
    }

    public class Borrower
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime MembershipDate { get; set; }

        public ICollection<Loan> Loans { get; set; } = new HashSet<Loan>();
    }

    public class Loan
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int BorrowerId { get; set; }
        public Borrower Borrower { get; set; }

        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class LibraryDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Borrower> Borrowers { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=LibraryDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Book>()
                .Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Borrower>()
                .Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Loan>()
                .HasKey(l => new { l.BookId, l.BorrowerId, l.LoanDate });

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Loan>()
                .HasOne(l => l.Borrower)
                .WithMany(b => b.Loans)
                .HasForeignKey(l => l.BorrowerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new LibraryDbContext();

            Console.WriteLine("Library System Initialized Successfully.");
            Console.WriteLine("Configured Entities:");
            Console.WriteLine($"- {nameof(Author)} (Id, Name, BirthDate)");
            Console.WriteLine($"- {nameof(Book)} (Id, Title, ISBN, AuthorId)");
            Console.WriteLine($"- {nameof(Borrower)} (Id, Name, MembershipDate)");
            Console.WriteLine($"- {nameof(Loan)} (BookId, BorrowerId, LoanDate, ReturnDate)");
            Console.WriteLine("Mapped Relationships:");
            Console.WriteLine("- One-to-Many: Author to Book");
            Console.WriteLine("- Many-to-Many: Book to Borrower through Loan");

            var script = context.Database.GenerateCreateScript();
            Console.WriteLine("\nGenerated Database DDL Script:");
            Console.WriteLine(script);
        }
    }
}
