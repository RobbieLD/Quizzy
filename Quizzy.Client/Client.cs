﻿using System;
using Quizzy.Common;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Quizzy.Client
{
    public class Client  : IControllable 
    {
        private TcpClient _client;
        private CancellationTokenSource _token;
        private int _score = 0;
        private Result _currentQuestion;

        public Client()
        {
        }

        public void AcceptCommand(string command)
        {
            switch (Enum.Parse<Commands>(command))
            {
                case Commands.CONNECT:
                    ConnectAsync();
                    break;
                case Commands.DISCONNECT:
                    Disconnect();
                    break;
                case Commands.SCORE:
                    Console.WriteLine($"Your score is {_score}");
                    break;
                case Commands.RESET:
                    _score = 0;
                    AcceptCommand(Commands.SCORE.ToString());
                    break;
                case Commands.A:
                case Commands.B:
                case Commands.C:
                case Commands.D:
                    AnswerQuestion(command);
                    break;
            }
        }

        public void Exit() => Disconnect();

        public bool RecogniseCommand(string command) => Enum.IsDefined(typeof(Commands), command);

        private void Disconnect()
        {
            if(_client != null && _client.Connected)
            {
                _token.Cancel();
            }
            else
            {
                Console.WriteLine("Client not connected");
            }
        }

        private void AnswerQuestion(string answer)
        {
            // don't let them have another go at the question
            if (string.IsNullOrWhiteSpace(_currentQuestion.correct_answer))
            {
                Console.WriteLine("You have already answered this questions");
                return;
            }

            if (_currentQuestion.correct_answer.ToUpper() == answer)
            {
                _score++;
                Console.WriteLine($"{Environment.NewLine} Correct. You're score is {_score}");
            }
            else
            {
                Console.WriteLine($"{Environment.NewLine} Wrong. The correct answer was {_currentQuestion.correct_answer}");
            }

            // null out the question answer so they can't answer it again
            _currentQuestion.correct_answer = string.Empty;
        }

        private async void ConnectAsync()
        {
            if (_client == null)
            {
                _client = new TcpClient();
            }
            else
            {
                if (_client.Connected) return;
            }

            _token = new CancellationTokenSource();
            _token.Token.Register(() => _client.Close());

            try
            {
                Console.WriteLine($"Connecting to server at {Constants.SERVER_IP}:{Constants.SERVER_PORT}");

                _client = new TcpClient();

                // Wait for the connection
                await _client.ConnectAsync(Constants.SERVER_IP, Constants.SERVER_PORT);

                HandleReadDataAsync(_client);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async void HandleReadDataAsync(TcpClient client)
        {
            try
            {
                object message = await client.ReadDataAsync(Constants.BUFFER_PREFIX_LENGTH);

                if(message is Result)
                {
                    Result question = message as Result;

                    question.Disaply();
                    _currentQuestion = question;
                }
                else
                {
                    // Just print the message to the console
                    Console.WriteLine($"Server says: {message}");
                }

                HandleReadDataAsync(client);

            }catch(IOException) when (_token.IsCancellationRequested)
            {
                // We can ignore the exception here as the client has closed the connecting so this read will fail
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}