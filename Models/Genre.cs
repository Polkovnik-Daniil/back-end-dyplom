using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Genre {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Book>? Books { get; set; }
        public IList<BookGenre>? BookGenre { get; set; }
    }
}
