namespace UsersApi.Helpers
{

    /// <summary>
    /// интерфейс, который реализуется в jwt сервисах
    /// </summary>
    public interface IJwtService
    {

        /// <summary>
        /// абстрактный метод генерации токена
        /// </summary>
        string GenerateJwtToken(string username);
    }
}
