namespace OnlineLibrary.DTOs
{
    public class Book
    {
        public Book(string title)
        {
            Title = title;
        }
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


    }
}
