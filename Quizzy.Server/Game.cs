using System;
using System.Collections.Generic;
using System.Text;
using Quizzy.Common;
using System.Net.Sockets;

namespace Quizzy.Server
{
    public class Game
    {
        private readonly QuestionsCollection _questions;
        private readonly IList<TcpClient> _players;

        public Game(QuestionsCollection questions, IList<TcpClient> players)
        {
            _players = players;
            _questions = questions;
        }

        public void Start()
        {
            foreach (Result question in _questions.results)
            {
                SendQuestionToClients(question);

                // Give them 10 seconds to answer
                System.Threading.Thread.Sleep(10000);
            }
        }

        private void SendQuestionToClients(Result question)
        {
            foreach(TcpClient client in _players)
            {
                client.SendQuestion(question);
            }
        }
    }
}
