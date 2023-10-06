using System.ComponentModel.DataAnnotations;

namespace UsersApi.Model.Dto
{
    /// <summary>
    /// Dto для создания пользователя, редактирования данных пользователя и получения токена
    /// </summary>
    public class DtoUser
    { 
        /// <summary> соответствует name из User </summary>
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        /// <summary> соответствует age из User </summary>
        [Required(ErrorMessage = "Age is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Age must be a positive number")]
        public int? Age { get; set; }

        /// <summary> соответствует email из User  </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
    }
}
