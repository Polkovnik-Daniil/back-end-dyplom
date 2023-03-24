namespace Models.garbage
{
    //В наличии класса нет необходимости в случаи использования ICollection<>
    public class BookAuthor
    {
        #region Fields
        public Book Book { get; set; }
        public Author Author { get; set; }
        #endregion
    }
}
