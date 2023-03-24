using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Author {
        #region Fields
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; } //

        #endregion
    }
}
