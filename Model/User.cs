using System.Data;
using System.Text.Json.Serialization;

namespace UsersApi.Model
{
    /// <summary>
    /// пользователь
    /// </summary>
    public class User
        {
        /// <summary>
        /// id пользователя
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// имя пользователя
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// возраст пользователя
        /// </summary>
        public int? Age { get; set; }
        /// <summary>
        /// email пользователя
        /// </summary>
        public string? Email{ get; set; }
        /// <summary>
        /// роли пользователя
        /// </summary>
        public List<UserRole>? userRoles { get; set; }
        }
}
