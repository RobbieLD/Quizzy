using System;
using Quizzy.Services;
using System.Net.Sockets;
using System.Net;
using Quizzy.Common;
using System.Threading;
using System.Collections.Generic;

namespace Quizzy.Server
{
    
    public class Server : IControllable
    {
        private readonly TcpListener _listener;
        private readonly IQuestionService _questionService;
        private CancellationTokenSource _token;
        private List<TcpClient> _clients;

        public Server(IQuestionService questionService)
        {
            _questionService = questionService;
            _listener = new TcpListener(IPAddress.Parse(Constants.SERVER_IP), Constants.SERVER_PORT);
        }

        #region Controllable Interface

        public void ShowHelp()
        {
            foreach(var command in Enum.GetNames(typeof(Commands)))
            {
                Console.WriteLine(command);
            }
        }

        public void AcceptCommand(string command)
        {
            switch(Enum.Parse<Commands>(command))
            {
                case Commands.START:
                    Start();
                    break;
                case Commands.STOP:
                    Stop();
                    break;
                case Commands.GAME:
                    StartNewGame(Constants.NUMBER_OF_QUESTIONS);
                    break;
                case Commands.CLIENTS:
                    DisplayClients();
                    break;
            }
        }

        public bool RecogniseCommand(string command)  => Enum.IsDefined(typeof(Commands), command);

        public void Exit() => Stop();

        #endregion

        #region Client IO Methods

        private void DisplayClients()
        {
            if (_clients == null || _clients.Count == 0)
            {
                Console.WriteLine("there are no clients connected");
            }else
            {
                _clients.ForEach(c => Console.WriteLine($"Client Connected: {c.GetHashCode()}"));
            }
        }

        private void Start()
        {
            try
            {
                _token = new CancellationTokenSource();
                _clients = new List<TcpClient>();
                _listener.Start();

                Console.WriteLine($"Server started on {Constants.SERVER_IP}:{Constants.SERVER_PORT}");

                HandleConnectionAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void Stop()
        {
            // Cancel all the listeners still waiting for connections
            if (_token != null)
            {
                if(_token.IsCancellationRequested)
                {
                    Console.WriteLine("Server already stopping");
                }
                else
                {
                    Console.WriteLine("Stopping Server ...");
                    _token.Cancel();
                    _listener.Stop();
                }
            }else
            {
                Console.WriteLine("Server not started");
            }
        }

        private async void StartNewGame(int questions)
        {
            Console.WriteLine($"Starting new game with {questions} questions");

            // Get the Questions
            QuestionsCollection questionsCollection = await _questionService.GetQuestions(questions);

            // Hand off running of the game to the game class
            Game game = new Game(questionsCollection, _clients);

            game.Start();

        }

        private async void HandleConnectionAsync()
        {
            while (!_token.IsCancellationRequested)
            {
                try
                {
                    // Wait here until the client to connect
                    TcpClient client = await _listener.AcceptTcpClientAsync().WithCancellation(_token.Token);

                    // Add the client to the list of connected clients
                    _clients.Add(client);

                    Console.WriteLine($"Client Connected: {client.GetHashCode()}");

                    // Send the welcome message
                    client.SendMessage("Welcome!\n");

                    HandleReadDataAsync(client);

                    // Go back to listning for clients
                    HandleConnectionAsync();

                }
                catch (OperationCanceledException) when (_token.IsCancellationRequested)
                {
                    // if we've requested a cancelation we can safly ignore thing
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void ReapClient(TcpClient client)
        {
            Console.WriteLine($"Client disconnect: {client.GetHashCode()}");
            _clients.Remove(client);
            client.Close();
        }

        private async void HandleReadDataAsync(TcpClient client)
        {
            using (client)
            {
                while (!_token.IsCancellationRequested)
                {
                    object message = await client.ReadDataAsync(Constants.BUFFER_PREFIX_LENGTH);

                    if(message == null)
                    {
                        // nothing on the stream to read so the client must have disconnected
                        ReapClient(client);
                        return;
                    }

                    Console.WriteLine($"Client says: {message}");
                    
                    HandleReadDataAsync(client);
                }
            }
        }

        #endregion
    }
}
