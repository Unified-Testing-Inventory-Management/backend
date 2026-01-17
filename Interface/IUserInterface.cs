using System;

namespace Server.Interface;

using Server.DTOs;
using Server.Models;

public interface IUserInterface
{
    Task RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs);
    Task LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs);
    Task<User?> GetUserById(string? userId);
}
