using System.Collections.Generic;
using System.Threading.Tasks;
using Quizzy.Common;

namespace Quizzy.Services
{
    public class MockQuestionService : IQuestionService
    {
        public Task<QuestionsCollection> GetQuestions(int number)
        {
            return Task.FromResult<QuestionsCollection>(new QuestionsCollection
            {
                results = new List<Result>
                {
                    new Result
                    {
                        question = "What year was the moon landing",
                        category = "History",
                        difficulty = "easy",
                        type = "multichoise",
                        correct_answer = "1969",
                        incorrect_answers = new List<string>
                        {
                            "1999",
                            "1959",
                            "1979"
                        }
                    }
                }
            });
        }
    }
}
