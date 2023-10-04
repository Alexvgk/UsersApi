namespace UsersApi.Model
{
    public class UserRole
    {
        public int userId { get; set; }
        public User user { get; set; }

        public int roleId { get; set; }
        public Role role { get; set; }
    }
}
