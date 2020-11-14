using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quizzy.Models;

namespace Quizzy.DataAccess
{
    public class QuizzyDBContext : DbContext
    {
        public DbSet<AuthUser> AuthUser { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source = quizzy.db");
        }
    }
}
