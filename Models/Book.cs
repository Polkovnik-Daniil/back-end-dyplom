using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Book {
        #region Fields
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? Realise { get; set; }
        public int? Count { get; set; }
        public int? Quantity { get; set; }
        public IList<Author>? Authors { get; set; } // BookAuthor
        public IList<BookAuthor>? BookAuthors { get; set; }
        public IList<Genre>? Genres { get; set; }   // BookGenre
        public IList<BookGenre>? BookGenre { get; set; }
        public IList<Reader>? Readers { get; set; } // BookReader
        public IList<BookReader>? BookReader { get; set; }
        #endregion
    }
}
