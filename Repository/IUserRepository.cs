using Microsoft.AspNetCore.Mvc;
using UsersApi.Model;
using UsersApi.Model.Dto;

namespace UsersApi.Repository
{

    /// <summary>
    /// интерфейс репозитория 
    /// </summary>
    public interface IUserRepository
    {

        Task<UsersPagedResult> getUser(string? filter, int page = 1, int pageSize = 10, string sortBy = "id", string sortOrder = "asc");
        Task<User> GetUserById(int id);
        Task<bool> AddRoleToUser(int userId, string? bodyRole);
        Task<int> CreateUser(DtoUser user);
        Task<bool> UpdateUser( int id,DtoUser updateUser);
        Task<bool> DeleteUser(int userId);
        Task<bool> DeleteRoleUser(int userId,string role);


    }
}
