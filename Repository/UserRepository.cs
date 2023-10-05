using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Nodes;
using UsersApi.Exeptions;
using UsersApi.Model;
using UsersApi.Service;

namespace UsersApi.Repository
{

    /// <summary>
    /// репозиторий получения данных из БД
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;


        public UserRepository(UserContext userContext)
        {
            _context = userContext; 
        }

        public async Task<bool> AddRoleToUser(int userId, string? bodyRole)
        {
            try
            {

                var user = await _context.users.Include(u => u.userRoles).FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new ArgumentNullException($"No one users with id {userId}");
                }

                var role = await _context.roles.FirstOrDefaultAsync(r => r.Name == bodyRole);

                if (role == null)
                {
                    throw new ArgumentNullException("No such role");
                }


                if (!(user.userRoles.IsNullOrEmpty()) && user.userRoles.Any(ur => ur.roleId == role.Id))
                {
                    throw new InvalidOperationException("This user has this role");
                }

                // new role
                var userRole = new UserRole
                {
                    userId = user.Id,
                    roleId = role.Id
                };

                _context.UserRoles.Add(userRole);


                await _context.SaveChangesAsync();
                return true;

            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex}");
            }
        }

        public async Task<int> CreateUser(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException("User can't be null");
                }
                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Name))
                {
                    throw new ArgumentNullException("Not all fields are filled in");
                }
                if (user.Age <= 0)
                {
                    throw new ArgumentException("Not acceptable age");
                }
                var existingUser = await _context.users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email is already in use");
                }

                _context.users.Add(user);
                await _context.SaveChangesAsync();
                return user.Id;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;

            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex}");

            }
        }

        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.users
                    .Include(u => u.userRoles)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new NoUserExeption("No user with this id");
                }

                if(!(user.userRoles.IsNullOrEmpty()))
                _context.UserRoles.RemoveRange(user.userRoles);

                _context.users.Remove(user);

                await _context.SaveChangesAsync();

                return true; 
            }
            catch (NoUserExeption ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex}");
            }
        }

        public async Task<UsersPagedResult> getUser(string? filter, int page, int pageSize, string sortBy, string sortOrder)
        {
            try
            {
                var query = _context.users.Include(u => u.userRoles).ThenInclude(r => r.role).AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    query = query.Where(u =>
                        u.Name.Contains(filter) || u.Email.Contains(filter) || u.userRoles.Any(ur => ur.role.Name.Contains(filter))
                    );
                }

                if(query.Count() == 0)
                {
                    throw new ArgumentNullException("No users with filter params");
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
                    var result = new UsersPagedResult
                    {
                        TotalItems = totalItems,
                        TotalPages = totalPages,
                        CurrentPage = currentPage,
                        PageSize = pageSize,
                        Users = users
                    };
                    
                    return result;
            }
            catch(ArgumentNullException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex}");
            }
        }

        public async Task<User> GetUserById(int id)
        {
            try
            {

                var user = await _context.users
                    .Include(u => u.userRoles)
                    .SingleOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new ArgumentNullException($"No users with id {id}");
                }

                return user;
            }
            catch (ArgumentNullException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex}");
            }
        }

        public async Task<bool> UpdateUser(int id, User updatedUser)
        {
            try
            {
                var user = await _context.users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new ArgumentNullException($"No users with id {id}");
                }

                // new data
                user.Name = updatedUser.Name;
                user.Age = updatedUser.Age;
                user.Email = updatedUser.Email;


                await _context.SaveChangesAsync();

                return true;
            }
            catch(ArgumentNullException ex)
            {
                throw ex;
            }
            catch(Exception ex) 
            {
                throw new Exception($"Internal server error: {ex}");
            }
        }

        public async Task<bool> DeleteRoleUser(int userId, string role)
        {
            try
            {
                var user = await _context.users.Include(u => u.userRoles).ThenInclude(r => r.role).SingleOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new NoUserExeption($"No users with id {userId}");
                }


                var roleToRemove = user.userRoles.SingleOrDefault(ur => ur.role.Name == role);

                if (roleToRemove == null)
                {
                    throw new ArgumentNullException($"No role {role}");
                }

               
                user.userRoles.Remove(roleToRemove);
                await _context.SaveChangesAsync();

                return true; 
            }
            catch(NoUserExeption ex)
            {
                throw ex;
            }
            catch(ArgumentNullException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex}");
            }
        }
    }
}
