using System.ComponentModel.DataAnnotations;


namespace Models {
    public class User {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public UserLogin UserLogin { get; set; }
        public string? Token { get; set; }
    }
}
