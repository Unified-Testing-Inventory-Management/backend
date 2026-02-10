using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Exceptions;
using Server.Interface;
using Server.Models;

namespace Server.Services;

public class UserServices(AppDbContext appDb, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, CurrentUserServices currentUserServices) : IUserService
{

    private readonly AppDbContext _db = appDb;

    private readonly IHttpContextAccessor _ihttpContextAccessor = httpContextAccessor;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly CurrentUserServices _currentServices = currentUserServices;
    public async Task RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        var user = await _userRepository.GetUser(_registerUserDTOs.Username);

        if (user is not null)
            throw new UserExceptions.UserAlreadyExists($"{_registerUserDTOs.Username} is already exists", 409);

        await _userRepository.SaveUser(_registerUserDTOs);
    }
    public async Task LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
    {
        var user = await _userRepository.GetUser(_loginUserDTOs.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(_loginUserDTOs.Password, user.Password))
            throw new UserExceptions.UnAuthorizedUserException("Username or password is incorrect", 401);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, _loginUserDTOs.Username),
        new Claim(ClaimTypes.Role, user.Role!)
    };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _ihttpContextAccessor.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );
    }
    public async Task<User?> GetUserById(string? userId)
    {
        return await _userRepository.GetUserId(userId) ?? await Task.FromResult<User?>(null); ;
    }
    public async Task UpdateUser(UserDTOs.UpdateUserDTOs _updateUserDTOs)
    {
        var userId = _currentServices.GetLoggedInUser();
        var user = await _userRepository.FindUserById(userId) ?? throw new UserExceptions.UserNotFoundException($"User Id {userId} not found", 404);

        await _userRepository.UpdateUser(user, _updateUserDTOs);

    }
}
