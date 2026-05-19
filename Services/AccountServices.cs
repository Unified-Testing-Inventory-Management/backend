using System;
using Server.Interface;
using Server.Models;
using Server.Repositories;

namespace Server.Services;

public class AccountServices(AccountRepository accountRepository) : IAccountService
{
    private readonly AccountRepository _accountRepository = accountRepository;

    public Task SaveAccount(Account account)
    {
        throw new NotImplementedException();
    }
}