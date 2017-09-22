using System;
using Quizzy.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Quizzy.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Hook up the DI
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddScoped<IControllable, Client>()
                .AddScoped<IShell, Shell>()
                .BuildServiceProvider();

            // Create the server
            IShell server = serviceProvider.GetService<IShell>();

            // Start the game server shell
            server.Start();
        }
    }
}
