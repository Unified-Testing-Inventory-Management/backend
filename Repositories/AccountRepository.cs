using System;
using Server.Class;
using Server.Data;
using Server.DTOs;
using Server.Interface;
using Server.Models;

namespace Server.Repositories;

public class AccountRepository(AppDbContext appDb) : IAccountRepository
{
    private readonly AppDbContext _db = appDb;

    public async Task SaveAccount(string AccountName)
    {
        var id = Guid.NewGuid();

        var account = new Account
        {
            Id = id,
            AccountName = AccountName,
            Role = Enums.UserRole.Admin.ToString(),
            DateOnly = DateOnly.FromDateTime(DateTime.Now)
        };

        await _db.SaveChangesAsync(account);
    }
}