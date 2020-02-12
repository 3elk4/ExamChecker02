using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.ImageProcessing
{

    /// <summary>
    /// Class detecting student's answers from table.
    /// </summary>
    /// <remarks> Uses segmented image and answers' position of boxes on image. </remarks>
    class StudentAnswersSeeker
    {
        /// <value> Detected table inside image. </value>
        private Image<Gray, byte> table = null;

        private Dictionary<int, List<int>> studentAnswers = null;
        /// <value> Student's answer structure. Contains question number as key and list of given answers as value.  </value>
        public Dictionary<int, List<int>> StudentAnswers { get => studentAnswers; }

        /// <value> Limit for answer detection. </value>
        private const float limit = 0.5F;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks> Makes thresholded image. </remarks>
        /// <param name="image"> Image of table. </param>
        public StudentAnswersSeeker(Image<Gray,byte> image)
        {
            table = new Image<Gray, byte>(image.Bitmap);
            setThresholded();
            studentAnswers = new Dictionary<int, List<int>>();
        }


        /// <summary>
        /// Makes thresholded table.
        /// </summary>
        private void setThresholded()
        {
            CvInvoke.Threshold(table, table, 100, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);
        }

        /// <summary>
        /// Method setting student's answer based on segmented image and positions of answers' bounding-boxes.
        /// </summary>
        /// <remarks> Ratio of white to black pixels gives information about student's answer. If no answer detected - list is empty. </remarks>
        /// <param name="questionAnswers"> Table structure. Contains question number as key and list of positions of answers' bonding-boxes as value. </param>
        public void setStudentAnswers(Dictionary<int, List<Rectangle>> questionAnswers)
        {
            var tableMat = table.Mat;
            
            foreach (var key in questionAnswers.Keys)
            {
                var boxes = questionAnswers[key];
               
                var answerList = new List<int>();

                for( int i = 0; i < boxes.Count; ++i)
                {
                    var temp = new Mat(tableMat, boxes[i]);
                    var whitePix = CvInvoke.CountNonZero(temp);

                    if(((double)whitePix / temp.Total.ToInt32()) >= limit)
                    {
                        answerList.Add(i);
                    }
                    
                }
                studentAnswers.Add(key, answerList);

            }
        }
    }
}
