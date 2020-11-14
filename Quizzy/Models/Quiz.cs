using Medallion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Quizzy.Models
{
    public class Quiz
    {
        public string question { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }
        public string category { get; set; }
        public string correct_answer { get; set; }
        public List<string> incorrect_answers { get; set; }
        [JsonIgnore]
        public DateTime End { get; set; }

        
        public List<string> GetAnswers()
        {
            List<string> answers = new List<string>();
            answers.Add(correct_answer);
            foreach(string s in incorrect_answers)
            {
                answers.Add(s);
            }
            answers.Shuffle();
            return answers;
        }
        
        public override string ToString()
        {
            return $"question: {question}, answers: {GetAnswers().ToString()}";
        }

        public static int GetPoint(string str)
        {
            int p = 0;
            switch (str)
            {
                case "easy": p=100;break;
                case "medium": p = 200; break;
                case "hard": p = 300; break;
            }
            return p;
        }
    }

    public class Result
    {
        public int response_code { get; set; }
        public List<Quiz> results { get; set; }

        public override string ToString()
        {
            return $"response_code: {response_code}, result: {results[0]}";
        }
    }
}
