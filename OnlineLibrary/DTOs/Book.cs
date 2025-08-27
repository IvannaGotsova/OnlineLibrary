namespace OnlineLibrary.DTOs
{
    public class Book
    {
        public Book(int id, string title, string description, string author, DateTime releaseDate, Genre genre, int pages, decimal price, string imageUrl)
        {
            Id = id;
            Title = title;
            Description = description;
            Author = author;
            ReleaseDate = releaseDate;
            Genre = genre;
            Pages = pages;
            Price = price;
            ImageUrl = imageUrl;
        }

        public int Id { get; set; }
        public string Title { get; set; }   
        public string Description { get; set; } 
        public string Author { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Genre Genre { get; set; }
        public int Pages { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }


    }
}
