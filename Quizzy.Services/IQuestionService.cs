using System.Threading.Tasks;
using Quizzy.Common;

namespace Quizzy.Services
{
    public interface IQuestionService
    {
        Task<QuestionsCollection> GetQuestions(int number);
    }
}