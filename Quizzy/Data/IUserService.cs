using Quizzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzy.Data
{
    public interface IUserService
    {
        Task<UserInfo> ValidateUserAsync(string Username, string Password);
        UserInfo GetUser();
        Task<List<UserInfo>> UpdateScore(bool won, string diff, string id);
        List<UserInfo> GetLeaderboard();
        Task<bool> RegisterAsync(string Username, string Password);
    }
}
