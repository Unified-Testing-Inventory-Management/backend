using System;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;

namespace Server.Repositories;

public class UserRepository(AppDbContext appDb) : IUserRepository
{
    private readonly AppDbContext _db = appDb;

    public async Task<User?> FindUserById(Guid userId)
    {
        return await _db.Users.FirstOrDefaultAsync(c => c.Id == userId);
    }

    public async Task<User?> GetUser(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(c => c.Username == username);
    }

    public async Task<User?> GetUserId(string? userId)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
    }

    public async Task SaveUser(UserDTOs.RegisterUserDTOs _registerUserDTOs)
    {
        var Id = Guid.NewGuid();

        var saveUser = new User
        {
            Id = Id,
            FirstName = char.ToUpper(_registerUserDTOs.FirstName[0]) + _registerUserDTOs.FirstName.Substring(1).ToLower(),
            LastName = char.ToUpper(_registerUserDTOs.LastName[0]) + _registerUserDTOs.LastName.Substring(1).ToLower(),
            Username = _registerUserDTOs.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(_registerUserDTOs.Password),
            Role = "Admin",
            CreatedAt = DateOnly.FromDateTime(DateTime.Now)
        };

        await _db.Users.AddAsync(saveUser);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateUser(User user, UserDTOs.UpdateUserDTOs _updateUserDTOs)
    {
        user.FirstName = _updateUserDTOs.FirstName;
        user.LastName = _updateUserDTOs.LastName;
        user.Username = _updateUserDTOs.Username;

        await _db.SaveChangesAsync();
    }
}
