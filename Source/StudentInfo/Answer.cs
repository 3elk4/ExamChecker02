using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.StudentInfo
{
    class Answer
    {
        public int AnswerNumber { get; set; }
        public Boolean Marked { get; set; }
        public Answer(int ANumber, Boolean isMarked)
        {
            this.AnswerNumber = ANumber;
            this.Marked = isMarked;
        }
    }
}
