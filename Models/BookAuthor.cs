namespace Models
{
    //В наличии класса нет необходимости в случаи использования ICollection<>
    public class BookAuthor
    {
        #region Fields
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
        #endregion
    }
}
