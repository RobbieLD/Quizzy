using Microsoft.Extensions.DependencyInjection;
using System;
using Quizzy.Services;
using Quizzy.Common;


namespace Quizzy.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Hook up the DI
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<IQuestionService, QuestionService>()
                .AddScoped<IControllable, Server>()
                .AddScoped<IShell, Shell>()
                .BuildServiceProvider();

            // Create the server
            IShell serverShell = serviceProvider.GetService<IShell>();

            // Start the game server shell
            serverShell.Start();
        }
    }
}
