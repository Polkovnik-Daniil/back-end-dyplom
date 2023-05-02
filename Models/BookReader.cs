namespace Models
{
    //
    public class BookReader
    {
        #region Fields
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int ReaderId { get; set; }
        public Reader? Reader { get; set; }
        public DateTime? DateTimeStart { get; set; }
        public DateTime? DateTimeEnd { get; set; }
        #endregion
    }
}
