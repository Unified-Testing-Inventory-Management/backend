using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface IUserRepository
{
    Task<User?> GetUser(string username);
    Task<User> SaveUser(UserDTOs.RegisterUserDTOs registerUserDTOs);
    Task<User?> GetUserId(string? userId);
    Task<User?> FindUserById(Guid userId);
    Task UpdateUser(User user, UserDTOs.UpdateUserDTOs _updateUserDTOs);
}
