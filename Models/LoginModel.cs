using System.ComponentModel.DataAnnotations;

namespace Models {
    public class LoginModel {
        [Required(ErrorMessage = "Invalid Email")]
        //[RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Invalid password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
