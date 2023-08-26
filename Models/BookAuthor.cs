namespace Models
{
    //В наличии класса нет необходимости в случаи использования ICollection<>
    public class BookAuthor
    {
        #region Fields

        // TODO : instead integer use GUID for ID
        public Guid BookId { get; set; }
        public Book? Book { get; set; }

        // TODO : instead integer use GUID for ID
        public Guid AuthorId { get; set; }
        public Author? Author { get; set; }
        #endregion
    }
}
