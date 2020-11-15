using Quizzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Timers;
using Quizzy.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Quizzy.Data
{
    public class QuizService
    {
        private Result Result;
        public Quiz qurrentQuiz;
        private Timer timer1;
        private long Category = 0;
        private string Difficulty = "0";
        private int Durration = 20;

        public event EventHandler SomethingHappened;

        private string TimeLeft;
        public QuizService()
        {
            getResult();
            InitTimer();
        }
        public void getResult()
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(getFetchUri()));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }

            Result = JsonConvert.DeserializeObject<Result>(jsonString);
            qurrentQuiz = Result.results[0];
            qurrentQuiz.End = DateTime.Now.AddSeconds(Durration + 1);
            Console.WriteLine("New question: " + qurrentQuiz.question);
        }
        public string getFetchUri()
        {
            string str = "https://opentdb.com/api.php?amount=1";
            
            if (Category != 0)
                str += "&category=" + Category;
            if (!Difficulty.Equals("0"))
                str += "&difficulty=" + Difficulty;
            return str;
        }
        public ResponseCategories getCategories()
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://opentdb.com/api_category.php"));

            WebReq.Method = "GET";

            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())   //modified from your code since the using statement disposes the stream automatically when done
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<ResponseCategories>(jsonString);
        }
        public void updateCategory(long cat)
        {
            Category = cat;
        }
        public long getCategory()
        {
            return Category;
        }
        public void updateDifficulty(string diff)
        {
            Difficulty = diff;
        }
        public string getDifficulty()
        {
            return Difficulty;
        }
        public void updateDurration(int dur)
        {
            Durration = dur;
        }
        public int getDurration()
        {
            return Durration;
        }
        public Quiz getQuizAsync() => qurrentQuiz;
        public string GetTimeLeft()
        {
            return TimeLeft;
        }

        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(timer1_Tick);
            timer1.Interval = 500; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = qurrentQuiz.End.Subtract(DateTime.Now);
            TimeLeft = ts.ToString("s''");
            SomethingHappened?.Invoke(this, EventArgs.Empty);
            if (TimeLeft.Equals("0"))
                getResult();
        }

        public bool Answer(string ans)
        {
            Console.WriteLine($"Guess: {ans}, Correct: {qurrentQuiz.correct_answer}");
            bool win = qurrentQuiz.correct_answer.Equals(ans);
            return win;
        }
    }
}
