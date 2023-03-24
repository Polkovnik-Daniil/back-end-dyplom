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
        public ICollection<Author> Authors { get; set; } //
        public ICollection<Genre> Genres { get; set; } //
        public ICollection<Reader> Readers { get; set; } //
        #endregion
    }
}
