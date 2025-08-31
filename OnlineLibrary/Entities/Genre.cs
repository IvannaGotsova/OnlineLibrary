namespace OnlineLibrary.Entities
{
    public class Genre
    {
        public int GenreId { get; set; }      
        public string Name { get; set; }

        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
