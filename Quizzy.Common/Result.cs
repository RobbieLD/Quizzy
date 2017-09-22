using System;
using System.Collections.Generic;
using System.Web;

namespace Quizzy.Common
{
    [Serializable]
    public class Result
    {
        public string category;
        public string type;
        public string difficulty;
        public string question;
        public string correct_answer;
        public List<string> incorrect_answers;

        private List<string> GetShuffledAnswers()
        {
            incorrect_answers.Add(correct_answer);
            incorrect_answers.Sort();

            char q = 'a';

            for(int i = 0; i < incorrect_answers.Count; i++)
            {
                //  Record the correct answer
                if(correct_answer == incorrect_answers[i])
                {
                    correct_answer = q.ToString();
                }

                incorrect_answers[i] = $"({q}) {incorrect_answers[i]}";
                q++;
            }

            return incorrect_answers;
        }

        public void Disaply()
        {
            Console.WriteLine(HttpUtility.HtmlDecode(question));

            foreach(string answer in GetShuffledAnswers())
            {
                Console.WriteLine(HttpUtility.HtmlDecode(answer));
            }
        }
    }
}
