namespace Models
{
    //
    public class BookReader
    {
        #region Fields

        // TODO : instead integer use GUID for ID
        public Guid BookId { get; set; }
        public Book? Book { get; set; }

        // TODO : instead integer use GUID for ID
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        // TODO : instead integer use GUID for ID
        public Guid ReaderId { get; set; }
        public Reader? Reader { get; set; }
        public DateTime? DateTimeStart { get; set; }
        public DateTime? DateTimeEnd { get; set; }
        #endregion
    }
}
