using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Reader {
        #region Fields
        [Key]

        // TODO : instead integer use GUID for ID
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public string? PlaceOfResidence { get; set; }
        public string PhoneNumber { get; set; }
        public IList<Book>? Books { get; set; }
        public IList<BookReader>? BookReader { get; set; } 
        #endregion
    }
}
