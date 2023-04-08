namespace Models
{
    //В наличии класса нет необходимости в случаи использования ICollection<>
    public class BookGenre
    {
        #region Fields
        public int BookId { get; set; }   
        /// <summary>
        /// Книга
        /// </summary>
        public Book Book { get; set; }
        public int GenreId { get; set; }
        /// <summary>
        /// Жанр книги
        /// </summary>
        public Genre Genre { get; set; }
        #endregion
    }
}
