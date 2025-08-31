namespace OnlineLibrary.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
