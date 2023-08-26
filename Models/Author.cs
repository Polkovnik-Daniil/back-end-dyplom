using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models {
    public class Author {
        #region Fields
        [Key]
        // TODO : instead integer use GUID for ID
        public Guid Id { get; set; }
        // TODO : instead name use patronomic and surname and name
        public string Name { get; set; }
        public IList<Book>? Books { get; set; } 
        public IList<BookAuthor>? BookAuthors { get; set; }
        #endregion
    }
}
