using System.Text.Json.Serialization;

namespace UsersApi.Model
{
    /// <summary>
    /// роль пользователя
    /// </summary>
    public class Role
    {
        /// <summary>
        /// id пользователя
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// роль 
        /// </summary>
        public string? Name { get; set; }
        [JsonIgnore]
        public List<UserRole>? userRole { get; set; }

    }
}
