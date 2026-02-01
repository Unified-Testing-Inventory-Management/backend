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

public class UserServices : IUserInterface
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _ihttpContextAccessor;
    public UserServices(IConfiguration configuration, AppDbContext appDb, IHttpContextAccessor httpContextAccessor)
    {
        _config = configuration;
        _db = appDb;
        _ihttpContextAccessor = httpContextAccessor;
    }

    public async Task RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        var user = _db.Users.FirstOrDefault(c => c.Username == _registerUserDTOs.Username);
        if (user != null)
            throw new UserExceptions.UserAlreadyExists(_registerUserDTOs.Username);

        var Id = Guid.NewGuid();

        var saveUser = new User
        {
            Id = Id,
            FirstName = char.ToUpper(_registerUserDTOs.FirstName[0]) + _registerUserDTOs.FirstName.Substring(1).ToLower(),
            LastName = char.ToUpper(_registerUserDTOs.LastName[0]) + _registerUserDTOs.LastName.Substring(1).ToLower(),
            Username = _registerUserDTOs.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(_registerUserDTOs.Password),
            Role = _registerUserDTOs.Role,
            CreatedAt = DateOnly.FromDateTime(DateTime.Now)
        };

        await _db.Users.AddAsync(saveUser);
        await _db.SaveChangesAsync();
    }

    public async Task LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
    {
        var user = await _db.Users.FirstOrDefaultAsync(c => c.Username == _loginUserDTOs.Username);

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

    public Task<User?> GetUserById(string? userId)
    {
        if (userId == null)
        {
            return Task.FromResult<User?>(null);
        }

        var user = _db.Users.FirstOrDefault(u => u.Id.ToString() == userId);
        return Task.FromResult(user);
    }
}
