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

        public async Task<List<UserInfo>> GetLeaderboard()
        {
            List<UserInfo> ui = await ctx.UserInfo.OrderByDescending(x => x.Score).Take(5).ToListAsync();
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
                UserInfo User = new UserInfo() { Id = Username, CorrectAnswers = 0, IncorrectAnswers = 0, Score = 0, SecurityLevel = 0 , LastQuestionAnswers = ""};

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

        public void Update(UserInfo u)
        {
            ctx.Update(u);
            Console.WriteLine("Updated");
        }

        public async Task SaveAsync()
        {
            await ctx.SaveChangesAsync();
            Console.WriteLine("Saved");
        }

        public async Task<List<UserInfo>> UpdateScore(bool won, string q, string diff, string id)
        {
            UserInfo user = await ctx.UserInfo.FirstAsync(u => u.Id.Equals(id));
            if (user.LastQuestionAnswers.Equals(q))
            {
                return null;
            }
            else if (won)
            {
                user.CorrectAnswers += 1;
                user.Score += Quiz.GetPoint(diff);
            }
            else
                user.IncorrectAnswers += 1;

            user.LastQuestionAnswers = q;
            ctx.Update(user);
            await ctx.SaveChangesAsync();

            this.user = user;
            return await GetLeaderboard();
        }

        public async Task<UserInfo> ValidateUserAsync(string key, string hash)
        {
            Console.WriteLine(hash);
            AuthUser first = await ctx.AuthUser.FirstAsync(a => a.Key.ToLower().Equals(key.ToLower()) && a.Hash.ToLower().Equals(hash.ToLower()));
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

        public async Task DeleteUser(string Id)
        {
            UserInfo ui= ctx.UserInfo.SingleOrDefault(x => x.Id.Equals(Id)); //returns a single item.
            AuthUser au= ctx.AuthUser.SingleOrDefault(x => x.Key.Equals(Id)); //returns a single item.

            if (ui != null && au != null)
            {
                ctx.UserInfo.Remove(ui);
                ctx.AuthUser.Remove(au);
            }
        }
    }
}
