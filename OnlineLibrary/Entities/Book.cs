using OnlineLibrary.DTOs;

namespace OnlineLibrary.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int Pages { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public IEnumerable<User> Users { get; set; } = new List<User>();
    }
}
