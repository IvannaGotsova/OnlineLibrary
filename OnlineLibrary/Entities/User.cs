using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Entities
{
    public class User
    {
        public User(int userId, string name)
        {
            UserId = userId;
            Name = name;
        }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 2)]
        public string Name { get; set; }
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
