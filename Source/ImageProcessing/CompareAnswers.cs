using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Class comparing student answers with key.
    /// </summary>
    class CompareAnswers
    {
        /// <value> Special number of exam. </value>
        private int IDexam;

        /// <values> Detected answers from <see cref="StudentAnswersSeeker"/>. </values>
        Dictionary<int, List<int>> studentAnswers;

        /// <value> Correct answers from key. </value>
        Dictionary<int, List<int>> correctAnswers;
        //get correct anwers from database

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="ID"> Special number of exam. </param>
        public CompareAnswers(int ID)
        {
            IDexam = ID;
            studentAnswers = new Dictionary<int, List<int>>();
            correctAnswers = new Dictionary<int, List<int>>();
        }

        
        /// <summary>
        /// Method getting result of test based on student's answers and key.
        /// </summary>
        /// <returns> Result in percentage. </returns>
        private float getResultOfTest()
        {
            float result = 0;

            foreach(var key in correctAnswers.Keys) {
                correctAnswers[key].ForEach(value => { if (studentAnswers[key].Contains(value)) result++; });
            }

            return (result / correctAnswers.Count) * 100;
        }

    }
}
