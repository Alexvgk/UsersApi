using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UsersApi.Exeptions;
using UsersApi.Model;
using UsersApi.Repository;
using UsersApi.Service;

namespace UsersApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        //all users 
        [HttpGet("/")]
        public async Task<IActionResult> GetUsers(string? filter = "", int page = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            try {
                var user = await _userRepository.getUser(filter, page, pageSize, sortBy, sortOrder);
                return Ok(user.Users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //get users by id and his roles
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                return Ok(user);
            }
            catch(ArgumentNullException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        //add new role for user
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] string? bodyRole)
        {
            try
            {
                var resp = await _userRepository.AddRoleToUser(userId, bodyRole);

                return Ok("Роль успешно добавлена пользователю.");
            }
            catch (ArgumentNullException ex) 
            {
                return StatusCode(404, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(407, ex.Message);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        ////add user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                int userId = _userRepository.CreateUser(user).Result;
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }

            catch(ArgumentNullException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch(ArgumentException ex)
            {
                return StatusCode(407, ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                return StatusCode(407, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //update user information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
              var resp = await _userRepository.UpdateUser(id, updatedUser);
              return Ok(resp);
            }
            catch(ArgumentNullException ex)
            {
                return StatusCode(404, $"Internal server error: {ex}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                bool resp = await _userRepository.DeleteUser(id);
                return NoContent(); //204 No Content 
            }
            catch(NoUserExeption e){ return StatusCode(404, e.Message); }
            catch (Exception ex){ return StatusCode(500, $"Internal server error: {ex}"); }

        }

    }
}
