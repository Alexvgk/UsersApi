using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UsersApi.Helpers;
using UsersApi.Model;
using UsersApi.Repository;

namespace UsersApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public TokenController(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }
        [HttpPost]
        public async Task<IActionResult> Post(User _userData){
            if (_userData != null && _userData.Email != null && _userData.Name != null)
            {
                var user = await _userRepository.GetUserById(_userData.Id);

                if (user != null && user.Name == _userData.Name && user.Email == _userData.Email)
                {
                    var token = _jwtService.GenerateJwtToken(_userData.Name);
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
