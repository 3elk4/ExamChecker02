using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.StudentInfo
{
    class Question
    {
        public int QuestionNumber {get; set;}
        public List<Answer> Answers { get; set; }
        public Question(int QNumber, List<Answer> answers)
        {
            this.QuestionNumber = QNumber;
            this.Answers = answers;
        }
    }
}
