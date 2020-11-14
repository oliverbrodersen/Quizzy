using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Quizzy.Models
{
    public class AuthUser
    {
        [Key]
        public string Key { get; set; }
        public string Hash { get; set; }

        public override string ToString()
        {
            return $"Key: {Key}, Hash: {Hash}";
        }
    }
}
