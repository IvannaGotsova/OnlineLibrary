namespace OnlineLibrary.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
