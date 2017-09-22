using System;
using Quizzy.Common;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;

namespace Quizzy.Services
{
    public class QuestionService : IQuestionService
    {
        private const string BASE_API_URL = "https://opentdb.com/";
        private const string API_PATH = "api.php?amount={0}";
        private readonly HttpClient _client;

        public QuestionService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(BASE_API_URL);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<QuestionsCollection> GetQuestions(int number)
        {
            QuestionsCollection questions = null;
            HttpResponseMessage response = await _client.GetAsync(string.Format(API_PATH, number));

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(QuestionsCollection));
                questions = (QuestionsCollection)serializer.ReadObject(stream);
            }

            return questions;
        }
    }
}
