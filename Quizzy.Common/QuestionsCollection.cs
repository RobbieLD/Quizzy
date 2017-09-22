using System;
using System.Collections.Generic;

namespace Quizzy.Common
{
    [Serializable]
    public class QuestionsCollection
    {
        public int response_code;
        public List<Result> results;  
    }
}
