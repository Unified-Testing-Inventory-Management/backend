using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.DTOs;
using Server.Exceptions;
using Server.Interface;
using Server.Models;

namespace Server.Services;

public class UserServices(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) : IUserService
{
    private readonly IHttpContextAccessor _ihttpContextAccessor = httpContextAccessor;
    private readonly IUserRepository _userRepository = userRepository;
    public async Task RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        var user = await _userRepository.GetUser(_registerUserDTOs.Username);

        if (user is not null)
            throw new UserExceptions.UserAlreadyExists(_registerUserDTOs.Username);

        await _userRepository.SaveUser(_registerUserDTOs);
    }
    public async Task LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
    {
        var user = await _userRepository.GetUser(_loginUserDTOs.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(_loginUserDTOs.Password, user.Password))
            throw new UserExceptions.UnAuthorizedUserException();

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
        if (userId is null)        
            return await Task.FromResult<User?>(null);

         return await _userRepository.GetUserId(userId);
    }
}
