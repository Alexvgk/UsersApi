namespace UsersApi.Helpers
{
    public interface IJwtService
    {
        string GenerateJwtToken(string username);
    }
}
