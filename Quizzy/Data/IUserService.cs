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
        Task SaveAsync();
        Task DeleteUser(string Id);
        void Update(UserInfo u);
        Task<List<UserInfo>> UpdateScore(bool won, string q, string diff, string id);
        Task<List<UserInfo>> GetLeaderboard();
        Task<bool> RegisterAsync(string Username, string Password);

    }
}
