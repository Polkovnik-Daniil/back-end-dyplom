using System.ComponentModel.DataAnnotations;

namespace Models {
    public class Role {
        [Key]

        // TODO : instead integer use GUID for ID
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
