using System.ComponentModel.DataAnnotations;

namespace Models {
    public class UserLogin {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public User User { get; set; } 
    }
}
