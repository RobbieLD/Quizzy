namespace Quizzy.Server
{
    internal enum Commands
    {
        /// <summary>
        /// Start the server
        /// </summary>
        START,
        
        /// <summary>
        /// Stop the server
        /// </summary>
        STOP,

        /// <summary>
        /// Start a new game
        /// </summary>
        GAME,

        /// <summary>
        /// Display list of connected clients
        /// </summary>
        CLIENTS
    }
}
