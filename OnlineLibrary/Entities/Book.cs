using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Entities
{
    public class Book
    {
        public Book(int bookId, string title, string description, int authorId, DateTime releaseDate, int genreId, int pages, decimal price, string imageUrl)
        {
            BookId = bookId;
            Title = title;
            Description = description;
            AuthorId = authorId;
            ReleaseDate = releaseDate;
            GenreId = genreId;
            Pages = pages;
            Price = price;
            ImageUrl = imageUrl;
        }

        [Required]
        public int BookId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Title { get; set; }
        [Required]
        [StringLength(10000, MinimumLength = 2)]
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        [Required]
        [Range(typeof(int), "1", "5000", ConvertValueInInvariantCulture = true)]
        public int Pages { get; set; }
        [Required]
        [Range(typeof(double), "0.00", "50000.00", ConvertValueInInvariantCulture = true)]
        public decimal Price { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string ImageUrl { get; set; }
        public ICollection<BookUser> UserBooks { get; set; } = new List<BookUser>();
    }
}
