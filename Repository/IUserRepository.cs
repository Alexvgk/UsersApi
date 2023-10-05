using Microsoft.AspNetCore.Mvc;
using UsersApi.Model;

namespace UsersApi.Repository
{
    public interface IUserRepository
    {

        Task<UsersPagedResult> getUser(string? filter, int page, int pageSize, string sortBy, string sortOrder);
        Task<User> GetUserById(int id);
        Task<bool> AddRoleToUser(int userId, string? bodyRole);
        Task<int> CreateUser(User user);
        Task<bool> UpdateUser( int id,User updateUser);
        Task<bool> DeleteUser(int userId);
        Task<bool> DeleteRoleUser(int userId,string role);


    }
}
