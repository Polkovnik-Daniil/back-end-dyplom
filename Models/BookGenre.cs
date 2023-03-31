namespace Models
{
    //В наличии класса нет необходимости в случаи использования ICollection<>
    public class BookGenre
    {
        #region Fields
        /// <summary>
        /// Книга
        /// </summary>
        public Book Book { get; set; }
        /// <summary>
        /// Жанр книги
        /// </summary>
        public Genre Genre { get; set; }
        #endregion
    }
}
