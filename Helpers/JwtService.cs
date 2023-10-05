using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UsersApi.Helpers
{
    /// <summary>
    ///jwt сервис
    /// </summary>
    public class JwtService : IJwtService
    {
        /// <summary>
        /// интерфейс доступа к конфигурационным файлам
        /// </summary>
        public IConfiguration _configuration;

        /// <summary>
        /// конструктор сервиса генерации токена
        /// </summary>
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// генерация токена
        /// </summary>
        /// <param name=username">пользователь, для которого генерируется токен</param>
        /// <returns></returns>
        public string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
