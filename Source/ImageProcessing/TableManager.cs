using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Class dealing with exam table.
    /// </summary>
    /// <remarks>
    /// Contains instance of <c>TableDetector</c> and <c>StudentAnswerSeeker</c>.
    /// </remarks>
    class TableManager
    {
        //temporary
        ///<value> Number of answers per question. </value>
        private const int answerNum = 4;
        ///<value> Number of questions in test. </value>
        private const int questionNum = 25;

        ///<value> Table image. </value>
        private Image<Gray, byte> table = null;
        
        ///<value> Table structure. Contains question number as key and list of positions of answers' bonding-boxes as value.</value>
        private Dictionary<int, List<Rectangle>> questionAnswersDict = null;

        ///<value> Instance of <see cref="StudentAnswersSeeker"/></value>
        private StudentAnswersSeeker studentAnswersSeeker = null;

        private PerfectModel perfectModel = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="image"> Main image after detecting aruco markers. </param>
        public TableManager(Image<Gray, byte> image)
        {
            table = new Image<Gray, byte>(image.Bitmap);
            questionAnswersDict = new Dictionary<int, List<Rectangle>>();
            studentAnswersSeeker = new StudentAnswersSeeker(table);
            perfectModel = new PerfectModel();
        }


        
        /// <summary>
        /// Main function managing process of getting answers from table.
        /// </summary>
        /// <remarks>
        /// Includes detecting table, detecting positions of answers' bounding-boxes and searching student's answers. 
        /// </remarks>
        public void mainProcess()
        {
            showTable();
           
            try { setAnswerBoxes(); }
            catch(InvalidNumberException) { throw; }
            catch(NullReferenceException) { throw; }
            catch(IndexOutOfRangeException) { throw; }

            studentAnswersSeeker.setStudentAnswers(questionAnswersDict);
            var studentAnswers = studentAnswersSeeker.StudentAnswers;

            foreach(var answerKey in studentAnswers.Keys) {
                Console.WriteLine("Question: " + answerKey + " , answer: ");
                studentAnswers[answerKey].ForEach(Console.WriteLine);
            }
            showTable();
        }

        /// <summary>
        /// Shows condition of table image after tranformations.
        /// </summary>
        /// <remarks>
        /// For testing .
        /// </remarks>
        public void showTable()
        {
            ImageViewer.Show(table, "table");
        }
        

        /// <summary>
        /// Operate on thresholded image to find contours ans detect position of answer boxes.
        /// </summary>
        /// <remarks>
        /// Contains <see cref="TableManager.findAllAnswerBoxes(Mat, VectorOfVectorOfPoint, out List{Rectangle})"/> 
        /// and <see cref="TableManager.initQuestionAnswerDictionary(List{Rectangle})"/>
        /// </remarks>
        private void setAnswerBoxes()
        {
            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            
            CvInvoke.FindContours(table, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            List<Rectangle> answerBoxes;
            findAllAnswerBoxes(hierarchy, contours, out answerBoxes);

            var temp = table.Copy();
            answerBoxes.ForEach(ab => { temp.Draw(ab, new Gray(255), 1);  });
            ImageViewer.Show(temp, "SQUARSES");

            if (!areElementsMatchingPattern(answerBoxes)) {
                throw new InvalidNumberException();
            }

            try {
                initQuestionAnswerDictionary(answerBoxes);
            }
            catch (NullReferenceException) {
                throw;
            }
            catch (IndexOutOfRangeException) {
                throw;
            }
            

            /*
            var inner = tableAfterMod.CopyBlank();
            for (int i = 1; i <= questionNum; ++i)
            {
                foreach(var rect in questionAnswersDict[i])
                {
                    inner.Draw(rect, new Gray(255), 1);
                    ImageViewer.Show(inner, "innertable");
                }
            }*/

        }



        /// <summary>
        /// Method finds bounding-boxes for answers' position.
        /// </summary>
        /// <remarks>
        /// Finding boxes based on approximation and <see cref="TableManager.isRectangle(VectorOfPoint)"/>
        /// </remarks>
        /// <param name="hierarchy"> Array made by <see cref="CvInvoke.FindContours(IInputOutputArray, IOutputArray, IOutputArray, RetrType, ChainApproxMethod, Point)"/>
        /// to pick only most internal contours. </param>
        /// <param name="contours"> Points describing contours made by 
        /// <see cref="CvInvoke.FindContours(IInputOutputArray, IOutputArray, IOutputArray, RetrType, ChainApproxMethod, Point)"/></param>
        /// <param name="rectangles"> Output bounding-boxes list. </param>
        private void findAllAnswerBoxes(Mat hierarchy, VectorOfVectorOfPoint contours, out List<Rectangle> rectangles)
        {
            rectangles = new List<Rectangle>();
            var data = hierarchy.GetData();
            var approxContour = new VectorOfPoint();


            for (int i = 0; i < hierarchy.Cols; ++i)
            {
                // [nextInHierarchy, previousInHierarchy, first child, parent]
                if (data.GetValue(0, i, 2).Equals(-1))
                {
                    var contour = contours[i];
                    CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                    if (isRectangle(approxContour))
                    {
                        rectangles.Add(CvInvoke.BoundingRectangle(approxContour));
                    }
                }
            }
        }



        /// <summary>
        /// Method initializing table structure - <see cref="TableManager.questionAnswersDict"/>
        /// </summary>
        /// <remarks>
        /// Before adding to dictionary - boxes are sorting top-to-bottom and left-to-right.
        /// </remarks>
        /// <param name="rectangles"> Bounding-boxes list. </param>
        private void initQuestionAnswerDictionary(List<Rectangle> rectangles)
        {
            try {
                var sorted = sortByQuestions(rectangles).ToList();

                for (int i = 1; i <= questionNum; ++i)
                {
                    questionAnswersDict.Add(i, sortByAnswers(sorted, i, answerNum).ToList());
                }
            }
            catch (NullReferenceException) {
                throw;
            }
            catch (IndexOutOfRangeException)
            {
                throw;
            }
        }

        /// <summary>
        /// Method sorting top-to-bottom answers' bounding-boxes.
        /// </summary>
        /// <exception cref="NullReferenceException"> When bounding-boxes list doesn't exist. </exception>
        /// <param name="rectangles"> Bounding-boxes list. </param>
        /// <returns> Sorted list by questions. </returns>
        private IOrderedEnumerable<Rectangle> sortByQuestions(List<Rectangle> rectangles)
        {
            if (rectangles == null) throw new NullReferenceException();
            return rectangles.OrderBy(b => b.Top).ThenBy(b => b.Left);
        }
       

        /// <summary>
        /// Method sorting left-to-right answers' bounding-boxes.
        /// </summary>
        /// <exception cref="NullReferenceException"> When bounding-boxes list doesn't exist.  </exception>
        /// <exception cref="IndexOutOfRangeException"> When number of question is not in section from 1 to questionNum or
        /// if number of answers is less or equal 0. </exception>
        /// <param name="rectangles"> Bounding-boxes list. </param>
        /// <param name="questNum"> Number of question from table. </param>
        /// <param name="answerNum"> Number of answers in question. </param>
        /// <returns> Sorted list of answer's bounding-boxes in given question. </returns>
        private IOrderedEnumerable<Rectangle> sortByAnswers(List<Rectangle> rectangles, int questNum, int answerNum)
        {
            if (rectangles == null) throw new NullReferenceException();
            if (questNum <= 0 || questNum > questionNum || answerNum <= 0) throw new IndexOutOfRangeException();

            var quest = rectangles.GetRange((questNum - 1) * answerNum, answerNum);
            return quest.OrderBy(b => b.Left);
        }


        /// <summary>
        /// Method checking if points create rectangle.
        /// </summary>
        /// <remarks> To define properly it uses number of vertices and angles' sizes. </remarks>
        /// <param name="approxCon"> Points' vector of contour after approximation. </param>
        /// <returns> True if is rectangle. </returns>
        private bool isRectangle(VectorOfPoint approxCon)
        {
            if (approxCon.Size == 4)
            {
                Point[] pts = approxCon.ToArray();
                LineSegment2D[] edges = PointCollection.PolyLine(pts, true);

                for (int j = 0; j < edges.Length; j++)
                {
                    double angle = Math.Abs(edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                    if (angle >= 80 && angle <= 100)
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Method checking number of detected answers' bounding-boxes.
        /// </summary>
        /// <remarks> The number must match the actual quantity. </remarks>
        /// <param name="rectangles"> Bounding-boxes list. </param>
        /// <returns> True if number match actual quantity. </returns>
        private bool areElementsMatchingPattern(List<Rectangle> rectangles)
        {
            return rectangles.Count.Equals(questionNum * answerNum) ? true : false;
        }


    }

    /// <summary>
    /// Exception class defining an invalid value.
    /// </summary>
    public class InvalidNumberException : Exception
    {
        public InvalidNumberException()
        {

        }
    }
}



