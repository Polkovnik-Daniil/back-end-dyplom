using System.ComponentModel.DataAnnotations;

namespace Models {
    public class User {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Invalid Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsLocked { get; set; }        
    }
}
