using Microsoft.EntityFrameworkCore;
using Quizzy.DataAccess;
using Quizzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzy.Data
{
    public class UserService : IUserService
    {
        private UserInfo user;
        private QuizzyDBContext ctx;

        public UserService(QuizzyDBContext ctx)
        {
            this.ctx = ctx;
        }

        public List<UserInfo> GetLeaderboard()
        {
            List<UserInfo> ui = ctx.UserInfo.OrderByDescending(x => x.Score).Take(5).ToList();
            ui.OrderByDescending(u => u.Score);
            return ui;
        }
        public UserInfo GetUser()
        {
            return user;
        }

        public async Task<bool> RegisterAsync(string Username, string Password)
        {
            bool authChech = ctx.AuthUser.Any(u => u.Key.ToLower().Equals(Username.ToLower()));
            if(!authChech)
            {
                AuthUser auth = new AuthUser() { Key = Username, Hash = Password };
                UserInfo User = new UserInfo() { Id = Username, CorrectAnswers = 0, IncorrectAnswers = 0, Score = 0 };

                ctx.AuthUser.Add(auth);
                ctx.UserInfo.Add(User);
                await ctx.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User already exists");
            }
            return true;
        }

        public async Task<List<UserInfo>> UpdateScore(bool won, string diff, string id)
        {
            UserInfo user = await ctx.UserInfo.FirstAsync(u => u.Id.Equals(id));
            if (won)
            {
                user.CorrectAnswers += 1;
                user.Score += Quiz.GetPoint(diff);
            }
            else
                user.IncorrectAnswers += 1;

            ctx.Update(user);
            await ctx.SaveChangesAsync();
            return GetLeaderboard();
        }

        public async Task<UserInfo> ValidateUserAsync(string key, string hash)
        {
            AuthUser first = await ctx.AuthUser.FirstAsync(a => a.Key.ToLower().Equals(key.ToLower()) || a.Hash.ToLower().Equals(hash.ToLower()));
            if (first == null)
            {
                throw new Exception("User not found");
            }
            else
            {
                user = await ctx.UserInfo.FirstAsync(user => user.Id.ToLower().Equals(key.ToLower()));
                Console.WriteLine("Welcome, " + user.Id);
            }
            return user;
        }
    }
}
