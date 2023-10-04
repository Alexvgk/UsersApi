namespace UsersApi.Model
{
    public class Role
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<UserRole>? userRole { get; set; }

    }
}
