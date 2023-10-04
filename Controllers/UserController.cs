using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UsersApi.Model;
using UsersApi.Service;

namespace UsersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }

        //all users 
        [HttpGet("/")]
        public async Task<IActionResult> GetUsers(string? filter = "", int page = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc")
        {
            try
            {
                var query = _context.users.AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(u =>
                        u.Name.Contains(filter) || u.Email.Contains(filter)
                    );
                }

                //sort
                switch (sortBy)
                {
                    case "Name":
                        query = sortOrder == "asc" ? query.OrderBy(u => u.Name) : query.OrderByDescending(u => u.Name);
                        break;
                    case "Age":
                        query = sortOrder == "asc" ? query.OrderBy(u => u.Age) : query.OrderByDescending(u => u.Age);
                        break;
                    case "Email":
                        query = sortOrder == "asc" ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email);
                        break;
                    default:
                        query = sortOrder == "asc" ? query.OrderBy(u => u.Id) : query.OrderByDescending(u => u.Id);
                        break;
                }

                var totalItems = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                var currentPage = Math.Clamp(page, 1, totalPages);
                if (currentPage > 1)
                {
                    var users = await query
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                    var result = new
                    {
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        CurrentPage = currentPage,
                        PageSize = pageSize,
                        Users = users
                    };
                    return Ok(result);
                }
                else
                {
                    var users = await query
                        .ToListAsync();
                    var result = new
                    {
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        CurrentPage = currentPage,
                        PageSize = pageSize,
                        Users = users
                    };
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //get users by id and his roles
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                
                var user = await _context.users
                    .Include(u => u.userRoles) 
                    .SingleOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound($"No one users with id{id}");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }

        //add new role for user
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] string? bodyRole)
        {
            try
            {
                
                var user = await _context.users.Include(u => u.userRoles).FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound($"No one users with id{userId}");
                }

               
                var role = await _context.roles.FirstOrDefaultAsync(r => r.Name == bodyRole);

                if (role == null)
                {
                    return NotFound("No such role");
                }

                
                if (!(user.userRoles.IsNullOrEmpty()) && user.userRoles.Any(ur => ur.roleId == role.Id))
                {
                    return BadRequest("This user has this role");
                }

                // new role
                var userRole = new UserRole
                {
                    userId = user.Id,
                    roleId = role.Id
                };

                _context.UserRoles.Add(userRole);

             
                await _context.SaveChangesAsync();

                return Ok("Роль успешно добавлена пользователю.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //add user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User can't be null");
                }
                if(string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Name))
                {
                    return BadRequest("Not all fields are filled in");
                }
                if(user.Age <= 0)
                {
                    return BadRequest("Not acceptable age");
                }
                var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    return Conflict("This email is alredy use");
                }

                _context.users.Add(user);
                await _context.SaveChangesAsync();

                //201 Created
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //update user information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var user = await _context.users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound("No user with this id");
                }

                // new data
                user.Name = updatedUser.Name;
                user.Age = updatedUser.Age;
                user.Email = updatedUser.Email;

            
                await _context.SaveChangesAsync();

                return Ok("Information updated");
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
                var user = await _context.users
                    .Include(u => u.userRoles)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    return NotFound($"No one users with id{id}");
                }


                _context.UserRoles.RemoveRange(user.userRoles);


                _context.users.Remove(user);

                await _context.SaveChangesAsync();

                return NoContent(); //204 No Content 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");

            }
        }

    }
}
