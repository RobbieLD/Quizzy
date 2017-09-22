using System;
using System.Threading.Tasks;

namespace Quizzy.Common
{
    public class Shell : IShell
    {
        
        private readonly IControllable _controllable;
        
        public Shell(IControllable controllable)
        {
            _controllable = controllable;
        }

        public void Start()
        {
            Console.WriteLine(" **** " + _controllable.GetType().Name + " **** ");

            while (true)
            {
                // Wait here for a command
                string command = Console.ReadLine();

                command = command.ToUpper();

                // First handle the command internally.
                if (Enum.IsDefined(typeof(Commands), command))
                {
                    switch (Enum.Parse<Commands>(command))
                    {
                        case Commands.QUIT:
                            _controllable.Exit();
                            return;
                        case Commands.CLEAR:
                            Console.Clear();
                            break;
                        case Commands.HELP:
                            ShowHelp();
                            break;
                    }
                }
                else if (_controllable.RecogniseCommand(command))
                {
                    // Now hand the command off to the controllable
                    _controllable.AcceptCommand(command);
                }
                else
                {
                    // This isn't a recognised command so we ignore it.
                    Console.WriteLine($"{command} isn't a regonised command");
                }

                Console.WriteLine();
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("The following commands are supported. You'll have to guess what they do :)");
            foreach(var command in Enum.GetNames(typeof(Commands)))
            {
                Console.WriteLine(command);
            }
            _controllable.ShowHelp();
        }
    }
}
