using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;

namespace Server.Services;

public class UserServices : IUserInterface
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _ihhtpContextAccessor;
    public UserServices(IConfiguration configuration, AppDbContext appDb, IHttpContextAccessor httpContextAccessor)
    {
        _config = configuration;
        _db = appDb;
        _ihhtpContextAccessor = httpContextAccessor;
    }

    public async Task RegisterUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        var user = _db.Users.FirstOrDefault(c => c.Username == _registerUserDTOs.Username);
        if (user != null)
        {
            throw new Exception("User already exists");
        }

        var Id = Guid.NewGuid();

        var saveUser = new User
        {
            Id = Id,
            FirstName = _registerUserDTOs.FirstName,
            LastName = _registerUserDTOs.LastName,
            Username = _registerUserDTOs.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(_registerUserDTOs.Password),
            Role = _registerUserDTOs.Role,
            CreatedAt = DateOnly.FromDateTime(DateTime.Now)
        };

        await _db.Users.AddAsync(saveUser);
        await _db.SaveChangesAsync();
    }

    public async Task<string> LoginUser(UserDTOs.LoginUserDTOs _loginUserDTOs)
    {
        var user = await _db.Users.FirstOrDefaultAsync(c => c.Username == _loginUserDTOs.Username);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Username or password is incorrect");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(_loginUserDTOs.Password, user.Password);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Password is incorrect");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, _loginUserDTOs.Username),
            new Claim(ClaimTypes.Role, user.Role!),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

         var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
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
