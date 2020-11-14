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

        public event EventHandler SomethingHappened;

        private string TimeLeft;
        public QuizService()
        {
            getResult();
            InitTimer();
        }
        public void getResult()
        {
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format("https://opentdb.com/api.php?amount=1"));

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
            qurrentQuiz.End = DateTime.Now.AddSeconds(21);
            Console.WriteLine("New question: " + qurrentQuiz.question);
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
