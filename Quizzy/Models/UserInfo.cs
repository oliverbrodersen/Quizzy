using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzy.Models
{
    public class UserInfo
    {
        [Key]
        public string Id { get; set; }
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }

    }
}
