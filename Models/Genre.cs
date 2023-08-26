using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Genre {
        [Key]

        // TODO : instead integer use GUID for ID
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<Book>? Books { get; set; }
        public IList<BookGenre>? BookGenre { get; set; }
    }
}
