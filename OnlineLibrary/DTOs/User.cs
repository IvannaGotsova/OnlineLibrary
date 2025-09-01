namespace OnlineLibrary.DTOs
{
    public class User
    {
        public User(int userId, string name)
        {
            UserId = userId;
            Name = name;
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
