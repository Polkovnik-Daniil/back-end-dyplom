using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Book {
        #region Fields
        /// <summary>
        /// Id книги
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Название книги
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Дата публикации
        /// </summary>
        public DateTime? Realise { get; set; }
        /// <summary>
        /// Количество страниц
        /// </summary>
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
