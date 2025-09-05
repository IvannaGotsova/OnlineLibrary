using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Data;
using OnlineLibrary.Entities;

namespace OnlineLibrary.DBContext
{
    
    public class OnlineLibraryContext : DbContext
    {
        public OnlineLibraryContext(DbContextOptions<OnlineLibraryContext> options)
            : base(options)
        {

        }
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BookUser> UserBooks { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().HasData(
            
                new Genre { GenreId = 1, Name = "Autobiography" },
                new Genre { GenreId = 2, Name = "Biography" },
                new Genre { GenreId = 3, Name = "Fantasy" },
                new Genre { GenreId = 4, Name = "Historical Fiction" },
                new Genre { GenreId = 5, Name = "History" },
                new Genre { GenreId = 6, Name = "Horror" },
                new Genre { GenreId = 7, Name = "Literary Fiction" },
                new Genre { GenreId = 8, Name = "Memoir" },
                new Genre { GenreId = 9, Name = "Mystery" },
                new Genre { GenreId = 10, Name = "Philosophy" },
                new Genre { GenreId = 11, Name = "Romance" },
                new Genre { GenreId = 12, Name = "Science Nature" },
                new Genre { GenreId = 13, Name = "Science Fiction" },
                new Genre { GenreId = 14, Name = "Self-Help" },
                new Genre { GenreId = 15, Name = "Thriller" },
                new Genre { GenreId = 16, Name = "True Crime" }
            );

            modelBuilder.Entity<BookUser>()
                .HasKey(bu => new { bu.UserId, bu.BookId });

            modelBuilder.Entity<BookUser>()
                .HasOne(bu => bu.User)
                .WithMany(u => u.UserBooks)
                .HasForeignKey(bu => bu.UserId);

            modelBuilder.Entity<BookUser>()
                .HasOne(bu =>bu.Book)
                .WithMany(b => b.UserBooks)
                .HasForeignKey(bu => bu.BookId);


            base.OnModelCreating(modelBuilder);
        }

    }
}
