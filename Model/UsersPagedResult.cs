namespace UsersApi.Model
{
    public class UsersPagedResult
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<User>? Users { get; set; }
    }
}
