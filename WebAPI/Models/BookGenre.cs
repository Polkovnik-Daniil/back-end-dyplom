namespace Models
{
    //В наличии класса нет необходимости в случаи использования ICollection<>
    public class BookGenre
    {
        #region Fields

        // TODO : instead integer use GUID for ID
        public Guid BookId { get; set; }   
        /// <summary>
        /// Книга
        /// </summary>
        public Book Book { get; set; }

        // TODO : instead integer use GUID for ID
        public Guid GenreId { get; set; }
        /// <summary>
        /// Жанр книги
        /// </summary>
        public Genre Genre { get; set; }
        #endregion
    }
}
