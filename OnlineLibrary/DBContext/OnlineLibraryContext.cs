using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Entities;

namespace OnlineLibrary.DBContext
{
    public class OnlineLibraryContext : DbContext
    {
        public  OnlineLibraryContext(DbContextOptions<OnlineLibraryContext> options)
        : base(options)
        { 
        }
        public DbSet<Book> Books => Set<Book>();

        public DbSet<Genre> Genres => Set<Genre>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().HasData(
            
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Autobiography, Name = "Autobiography" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Biography, Name = "Biography" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Fantasy, Name = "Fantasy" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.HistoricalFiction, Name = "Historical Fiction" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.History, Name = "History" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Horror, Name = "Horror" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.LiteraryFiction, Name = "Literary Fiction" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Memoir, Name = "Memoir" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Mystery, Name = "Mystery" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Philosophy, Name = "Philosophy" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Romance, Name = "Romance" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.ScienceNature, Name = "Science Nature" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.ScienceFiction, Name = "Science Fiction" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.SelfHelp, Name = "Self-Help" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.Thriller, Name = "Thriller" },
                new Genre { Id = (int)OnlineLibrary.DTOs.Genre.TrueCrime, Name = "True Crime" }
            );
        }

    }
}
