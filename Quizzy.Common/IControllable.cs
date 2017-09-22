using System;
using System.Collections.Generic;
using System.Text;

namespace Quizzy.Common
{
    public interface IControllable
    {
        void AcceptCommand(string command);
        bool RecogniseCommand(string command);
        void Exit();
    }
}
