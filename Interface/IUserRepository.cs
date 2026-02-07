using System;
using Server.DTOs;
using Server.Models;

namespace Server.Interface;

public interface IUserRepository
{
    Task<User?> GetUser(string username);
    Task SaveUser(UserDTOs.RegisterUserDTOs registerUserDTOs);
    Task<User?> GetUserId(string userId);
}
