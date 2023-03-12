using System.Text.Json.Serialization;

namespace Models {
    public class User {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [JsonIgnore]
        public string Pwd { get; set; }
        public string? Token { get; set; }
        [JsonIgnore]
        public Role Role { get; set; }
        public User() { }
    }
}
