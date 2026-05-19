using System;
using Server.DTOs;

namespace Server.Interface;

public interface IAccountRepository
{
    Task SaveAccount(string Account);
}