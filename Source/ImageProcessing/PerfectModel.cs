using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// This class contains ideal model of answer table, based on actual measures of elements.  
    /// </summary>
    class PerfectModel
    {
        /// <value> Contains number of question as a key and vector of left-uppper corners for space for an answer. </value>
        public Dictionary<int, VectorOfPointF> squaresPosition;

        /// <value> Visual interpretation of ideal table. (for tests only!). </value>
        private Image<Gray, byte> perfectTableImage;

        /// <value> Size of space for question number. </value>
        //private PointF start = CardConstants.Instance.getNumericBoxSize();

        /// <value> Size of space for answer. </value>
        public PointF boxSize;// = CardConstants.Instance.getAnswerBoxSize();


        /// <summary>
        /// This constructor defines left-upper corners for each space for an answer.
        /// </summary>
        public PerfectModel()
        {
            PointF uLC = CardConstants.Instance.getUpperLeftCorner();
            PointF bRC = CardConstants.Instance.getBottomRightCorner();
            PointF size = CardConstants.Instance.getSizeOfAnswerChart();

            squaresPosition = new Dictionary<int, VectorOfPointF>();
            perfectTableImage = new Image<Gray, byte>(new Size((int)size.X, (int)size.Y));

            initVertices();
           foreach (var vec in squaresPosition.Values)
            {
                foreach(var v in vec.ToArray())
                    CvInvoke.Circle(perfectTableImage, new Point((int)v.X, (int)v.Y), 2, new MCvScalar(255));
            }

            ImageViewer.Show(perfectTableImage, "ITS JUST PERFECT!");
        }

        /// <summary>
        /// This method defines position of left-upper corners for each space for an answer.
        /// </summary>
        /// <remarks> Corners are defined based on image section where the answer table exists. </remarks>
        private void initVertices()
        {
            int questionNumber = CardConstants.Instance.getQuestionQuantity();
            int answerNumber = CardConstants.Instance.getAnswerPerQuestionQuantity();
            PointF size = CardConstants.Instance.getSizeOfAnswerChart();

            float h = size.Y / (questionNumber+1);
            PointF start = new PointF(0, h);
            float offset = size.X / answerNumber;

            boxSize = new PointF(offset, h);

            for(int i = 1; i <= questionNumber; ++i)
            {
                VectorOfPointF vectorOfPoint = new VectorOfPointF();
                vectorOfPoint.Push(new PointF[]
                {
                    new PointF(start.X, start.Y),
                    new PointF((int)(start.X + offset), (int)start.Y),
                    new PointF((int)(start.X + 2 * offset), (int)start.Y),
                    new PointF((int)(start.X + 3 * offset) , (int)start.Y)
                });
                squaresPosition.Add(i, vectorOfPoint);
                start.Y = start.Y + h;
               // start = new Point(start.X, (int)(start.Y + h));
            }
        }


    }
}
