namespace Quizzy.Common
{
    public interface IControllable
    {
        void AcceptCommand(string command);
        bool RecogniseCommand(string command);
        void Exit();
        void ShowHelp();
    }
}
