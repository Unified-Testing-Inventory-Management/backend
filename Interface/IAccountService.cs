using System;
using Server.Models;

namespace Server.Interface;

public interface IAccountService
{
    Task SaveAccount(Account account);
}