using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using UsersApi.Exeptions;
using UsersApi.Model;
using UsersApi.Model.Dto;
using UsersApi.Repository;
using UsersApi.Service;

namespace UsersApi.Controllers
{

    /// <summary>
    /// main контроллер
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// конструктор
        /// </summary>
        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        /// <param name="filter">фильтр, позволяет отбирать данные по имени, эл.почте или роли</param>
        /// <param name="page">страница вывода</param>
        /// <param name="pageSize">размер страницы</param>
        /// <param name="sortBy">сортировка по параметрам(id,name,email)</param>
        /// <param name="sortOrder">порядок сортировки</param>
        /// <returns></returns>
        [HttpGet("/")]
        [SwaggerResponse(StatusCodes.Status200OK, "User list", typeof(List<User>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetUsers(string? filter = "", int page = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            try {
                var user = await _userRepository.getUser(filter, page, pageSize, sortBy, sortOrder);
                _logger.LogInformation("GET USERS");
                return Ok(user.Users);
            }
            catch(ArgumentNullException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception in getting users");
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Поиск пользователя по id
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns></returns>
        //get users by id and his roles
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "User details", typeof(User))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                _logger.LogInformation("GET USER BY ID");
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

        /// <summary>
        /// Добавление роли пользователю
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="bodyRole">роль, котоую хотим добавить(Admin,User,Support,SuperAdmin)</param>
        /// <returns></returns>
        [HttpPost("{userId}/roles")]
        [SwaggerResponse(StatusCodes.Status200OK, "Role added")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] string? bodyRole)
        {
            try
            {
                var resp = await _userRepository.AddRoleToUser(userId, bodyRole);

                _logger.LogInformation($"ADDED ROLE TO USER {userId}");
                return Ok("Role added.");
            }
            catch (ArgumentNullException ex) 
            {
                return StatusCode(404, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(406, ex.Message);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="user">пользователь</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "User Created")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "no user")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "invalid data")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> CreateUser([FromBody] DtoUser user)
        {
            try
            {
                int userId = await _userRepository.CreateUser(user);
                _logger.LogInformation($"CREATE USER WITH ID {userId}");
                return Ok(userId);
            }


            catch (ArgumentNullException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(406, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        /// <param name="id">id пользователя, которому обновляем информацию/</param>
        /// <param name="updatedUser">новые данные пользователя</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] DtoUser updatedUser)
        {
            try
            {
              var resp = await _userRepository.UpdateUser(id, updatedUser);
                _logger.LogInformation($"USERS {id} UPDATED");
                return Ok(resp);
            }
            catch(ArgumentNullException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        /// <summary>
        /// удаление пользовалетя
        /// </summary>
        /// <param name="id">id пользователя, которого хотим удалить/</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "delete user")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                bool resp = await _userRepository.DeleteUser(id);
                _logger.LogInformation($"DELETE USER {id}");
                return NoContent(); //204 No Content 
            }
            catch(NoUserExeption e){ return StatusCode(404, e.Message); }
            catch (Exception ex){ return StatusCode(500, $"Internal server error: {ex}"); }

        }


        /// <summary>
        /// удаление роли у пользователя
        /// </summary>
        /// <param name="userId">id пользователя,у которого хотим удалить роль/</param>
        /// <param name="role">роль,которую хотим удалить/</param>
        /// <returns></returns>
        [HttpDelete("{userId}/roles")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "delete role")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Role not found")]
        public async Task<IActionResult> DeleteRoleUser(int userId,string role)
        {
            try
            {
                bool resp = await _userRepository.DeleteRoleUser(userId,role);
                _logger.LogInformation($"DELETE USER {userId}");
                return NoContent(); //204 No Content 
            }
            catch (NoUserExeption e) { return StatusCode(404, e.Message); }
            catch(ArgumentNullException e) { return StatusCode(404, e.Message); }
            catch (Exception ex) { return StatusCode(500, $"Internal server error: {ex}"); }

        }

    }
}
