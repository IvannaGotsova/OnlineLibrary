namespace OnlineLibrary.DTOs
{
    public class Author
    {
        public Author(int authorId, string name, string biography)
        {
            AuthorId = authorId;
            Name = name;
            Biography = biography;
        }

        public int AuthorId { get; set; } 
        public string Name { get; set; }
        public string Biography { get; set; }
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
