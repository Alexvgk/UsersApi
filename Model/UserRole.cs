using System.Text.Json.Serialization;

namespace UsersApi.Model
{
    /// <summary>
    /// связь пользователь-роль для many-to-many связи
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// id пользователя
        /// </summary>
        public int userId { get; set; }
       [JsonIgnore]
        public User user { get; set; }
        /// <summary>
        /// id роли
        /// </summary>
        public int roleId { get; set; }
        public Role role { get; set; }
    }
}
