using System.Text.Json.Serialization;

namespace UsersApi.Model
{
    public class UserRole
    {
        public int userId { get; set; }
       [JsonIgnore]
        public User user { get; set; }

        public int roleId { get; set; }
        public Role role { get; set; }
    }
}
