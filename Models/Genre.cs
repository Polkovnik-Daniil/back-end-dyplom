using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Genre {

        /// <summary>
        /// Id жанра 
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Название жанра
        /// </summary>
        public string Name { get; set; }
        public IList<Book>? Books { get; set; }
        public IList<BookGenre>? BookGenre { get; set; }
    }
}
