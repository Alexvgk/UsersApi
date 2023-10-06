using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Swashbuckle.AspNetCore.Annotations;
using UsersApi.Helpers;
using UsersApi.Model;
using UsersApi.Model.Dto;
using UsersApi.Repository;

namespace UsersApi.Controllers
{
    /// <summary>
    /// контроллер получения токена
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<TokenController> _logger;

        /// <summary>
        /// конструктор контроллера
        /// </summary>
        public TokenController(IUserRepository userRepository, IJwtService jwtService, ILogger<TokenController> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _logger = logger;
        }


        /// <summary>
        /// Получение токена
        /// </summary>
        /// <param name="_userData">пользователь, который получает токен</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Get token",typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "invalid credetials")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> Post(DtoUser _userData){
            if (_userData != null && _userData.Email != null && _userData.Name != null)
            {
                var result = await _userRepository.getUser(_userData.Email);
                var user = result.Users.FirstOrDefault();

                if (user != null && user.Name == _userData.Name && user.Email == _userData.Email)
                {
                    var token = _jwtService.GenerateJwtToken(_userData.Name);
                    _logger.LogInformation($"GET TOKEN TO {_userData.Name}");
                    return Ok(token);
                }
                else
                {
                    return StatusCode(401, "Invalid credentials");
                }
            }
            else
            {
                return StatusCode(404,"Invaid user");
            }
        }

    }

}
