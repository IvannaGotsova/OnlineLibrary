using System.ComponentModel.DataAnnotations;

namespace OnlineLibrary.Entities
{
    public class Author
    {
        public Author(int authorId, string name, string biography)
        {
            AuthorId = authorId;
            Name = name;
            Biography = biography;
        }

        [Required]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Biography { get; set; }
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
    }
}
