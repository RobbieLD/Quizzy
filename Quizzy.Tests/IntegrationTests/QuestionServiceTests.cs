using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Quizzy.Services;

namespace Quizzy.Tests.IntegrationTests
{
    public class QuestionServiceTests
    {
        [Fact]
        public void QuestionService_GetQuestions_GetsQuestions()
        {
            // Arrange
            int numQuestions = 10;

            QuestionService service = new QuestionService();

            // Act
            var questions = service.GetQuestions(numQuestions).Result;

            // Assert
            Assert.Equal(questions.results.Count, numQuestions);
        }
    }
}
