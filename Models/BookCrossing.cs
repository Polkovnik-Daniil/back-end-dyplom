namespace Models
{
    //
    public class BookCrossing
    {
        #region Fields
        public int Id { get; set; }
        public Book Book { get; set; }
        public Reader Reader { get; set; }
        #endregion
    }
}
