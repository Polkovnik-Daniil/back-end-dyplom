using System.ComponentModel.DataAnnotations;

namespace Models {
    public class User {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Invalid Email")]
        [EmailAddress]
        //[RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
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
