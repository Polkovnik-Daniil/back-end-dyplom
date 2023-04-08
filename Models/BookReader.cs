namespace Models
{
    //
    public class BookReader
    {
        #region Fields
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int ReaderId { get; set; }
        public Reader Reader { get; set; }
        public DateTime DateTime { get; set; }
        #endregion
    }
}
