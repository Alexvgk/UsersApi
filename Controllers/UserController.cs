using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersApi.Model;
using UsersApi.Service;

namespace UsersApi.Controllers
{
    public class UserController : Controller
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }

        //all users 
        [HttpGet]
        public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 10, string sortBy = "Id", string sortOrder = "asc", string filter = "")
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        //get users by id and his roles
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            return null;
     
        }

        //add new role for user
        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AddRoleToUser(int userId, [FromBody] Role role)
        {
            return null;
        }

        //add user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            return null;
        }

        //update user information
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            return null;
        }

        //delete user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return null;
        }

    }
}
