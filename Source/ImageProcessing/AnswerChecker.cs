using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Class checking box by box student's answers.
    /// </summary>
    class AnswerChecker
    {
        /// <value> Contains answer table (after transformation and thresholding). </value>
        private Image<Gray, byte> image = null;

        /// <value> Contains ideal model. </value>
        private PerfectModel perfectModel = null;

        /// <value> Contains student answers detected on the image. Key -> number of question, value -> list of given and ungiven answers. </value>
        private Dictionary<int, List<bool>> studentAnswers;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image"> Answer table after transformation. </param>
        public AnswerChecker(Image<Gray, byte> image)
        {
            this.image = new Image<Gray, byte>(image.Bitmap);
            perfectModel = new PerfectModel();
            studentAnswers = new Dictionary<int, List<bool>>();
        }

        /// <summary>
        /// This method write on the console number of question with given and ungiven answers.
        /// </summary>
        public void showStudentAnswers()
        {
            foreach(var kvp in studentAnswers)
            {
                Console.Write(kvp.Key + " -> ");
                kvp.Value.ForEach(v => Console.Write(v + " "));
                Console.WriteLine();
            }
        }

        public List<StudentInfo.Question> ConvertToStudentExamData()
        {
            List<StudentInfo.Question> answers = new List<StudentInfo.Question>();
            int i = 0;
            foreach (var kv in this.studentAnswers){
                List<StudentInfo.Answer> tempAnswers = new List<StudentInfo.Answer>();
                i = 0;
                foreach(var v in kv.Value) {
                    tempAnswers.Add(new StudentInfo.Answer(i, v));
                    i++;
                }
                answers.Add(new StudentInfo.Question(kv.Key-1, tempAnswers));
            }
            return answers;
        }


        /// <summary>
        /// This method detects answer from the image of answer table.
        /// </summary>
        /// <remarks> </remarks>
        public void checkAnswers()
        {
            foreach (var vec in perfectModel.squaresPosition)
            {
                List<bool> answers = new List<bool>();
                //Console.WriteLine("NUM: " + vec.Key);
            
                foreach (var lPoint in vec.Value.ToArray())
                {


                    #region CuttingAnswerBox
                    bool isAnswer = false;

                    Point point = new Point((int)Math.Round(lPoint.X), (int)Math.Round(lPoint.Y));
                    Rectangle roi = new Rectangle(point, new Size((int)perfectModel.boxSize.X, (int)perfectModel.boxSize.Y));
                    image.ROI = roi;
                    var answerImage = image.Copy();
                   
                  
                   //if (vec.Key == 9)
                       // ImageViewer.Show(answerImage, "answer image");

                    Point squareOffset = new Point((int)CardConstants.Instance.getSquareOffset().X, (int)CardConstants.Instance.getSquareOffset().Y);
                    Size squareBoxSize = new Size((int)CardConstants.Instance.getSquareBoxSize().X, (int)CardConstants.Instance.getSquareBoxSize().Y);

                    Rectangle squareRoi = new Rectangle(squareOffset, squareBoxSize);
                    answerImage.ROI = squareRoi;
                    var squareImage = answerImage.Copy();

                     //if (vec.Key == 6 || vec.Key == 9)
                      //ImageViewer.Show(squareImage, "answer image");

                    Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
                    CvInvoke.MorphologyEx(squareImage, squareImage, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                    Mat kernel1 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(1, 3), new Point(-1, -1));
                    CvInvoke.MorphologyEx(squareImage, squareImage, MorphOp.Dilate, kernel1, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                    Mat kernel2 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 1), new Point(-1, -1));
                    CvInvoke.MorphologyEx(squareImage, squareImage, MorphOp.Dilate, kernel2, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                    //if (vec.Key == 6 || vec.Key == 9)
                     //   ImageViewer.Show(squareImage, "answer image");

                    //2 iteration for better removing not very important elements
                    //CvInvoke.MorphologyEx(answerImage, answerImage, MorphOp.Close, new Mat(), new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                    //CvInvoke.MorphologyEx(answerImage, answerImage, MorphOp.Open, new Mat(), new Point(-1, -1), 2, BorderType.Default, new MCvScalar());

                    var total = squareImage.Mat;
                    var whitePix = CvInvoke.CountNonZero(total);
                    var limit = 0.8;
                    var ratio = (double)whitePix / total.Total.ToInt32();

                  //if (vec.Key == 6 || vec.Key == 9)
                       // Console.WriteLine("RATIO: " + ratio);



                    if (Math.Round(ratio, 1) >= limit)
                    {
                        isAnswer = true; 
                    }
                    answers.Add(isAnswer);



                    // if (vec.Key == 1)
                    // ImageViewer.Show(answerImage, "answer image");

                    // Mat kernel1 = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross, new Size(3, 3), new Point(1, 1));
                    //CvInvoke.MorphologyEx(answerImage, answerImage, MorphOp.Open, new Mat(), new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
                    // CvInvoke.MorphologyEx(answerImage, answerImage, MorphOp.Close, kernel1, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

                    //if(vec.Key == 1)
                    // ImageViewer.Show(answerImage, "answer image");

                    //Image<Gray, byte> tempROI = new Image<Gray, byte>(roi.Size);
                    #endregion CuttingAnswerBox

                    /*using (var contours = new VectorOfVectorOfPoint())
                    {
                        CvInvoke.FindContours(answerImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                        if(contours.Size > 0)
                        {
                            for(int i = 0; i < contours.Size; ++i)
                            {
                                using (var approxContour = new VectorOfPoint())
                                {
                                    CvInvoke.ApproxPolyDP(contours[i], approxContour, CvInvoke.ArcLength(contours[i], true) * 0.05, true);
                                    double area = CvInvoke.ContourArea(approxContour, false);
                                    // Console.WriteLine("key: " + vec.Key + ", answer: " + lPoint.X + "AREA: " + area + ", approxContourSize: " + approxContour.Size);
                                    #region check if contour is answer box
                                    if ((area > 80 && area < 110) && approxContour.Size == 4)
                                    {
                                        #region check if contour is rectangle
                                        bool isRectangle = true;
                                        Point[] pts = approxContour.ToArray();
                                        LineSegment2D[] edges = PointCollection.PolyLine(pts, true);
                                        RotatedRect tablePoints = new RotatedRect();
                                        PointF[] points = null;

                                        for (int j = 0; j < edges.Length; j++)
                                        {
                                            double angle = Math.Abs(
                                                edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));

                                            //Console.WriteLine("key: " + vec.Key + ", answer: " + lPoint.X + "angle: " + angle);
                                            if (angle < 75 || angle > 105)
                                            {
                                                isRectangle = false;
                                                break;
                                            }
                                        }

                                        //Console.WriteLine("SIZE: " + approxContour.Size + ", ANGLE: " + angle + ", IS RECTANGLE: " + isRectangle);
                                        #endregion
                                        if (isRectangle)
                                        {
                                            #region check if answer box is filled
                                            tablePoints = CvInvoke.MinAreaRect(approxContour);
                                            points = tablePoints.GetVertices();
                                            var sortedPoints = points.OrderBy(p => Math.Sqrt(p.X * p.X + p.Y * p.Y));
                                            var left = sortedPoints.First();
                                            var size = sortedPoints.Last();

                                            Rectangle innerRoi = new Rectangle((int)left.X, (int)left.Y, (int)(size.X - left.X), (int)(size.Y - left.Y));
                                            answerImage.ROI = innerRoi;

                                            var temp = answerImage.Copy();
                                            //if(vec.Key == 13 && lPoint.X == 43)
                                            //ImageViewer.Show(temp, "SQUAAARE");

                                            var total = temp.Mat;
                                            var whitePix = CvInvoke.CountNonZero(total);
                                            var limit = 0.5;

                                            if (((double)whitePix / total.Total.ToInt32()) >= limit)
                                            {
                                                isAnswer = true;
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion 
                                }
                            }
                        }
                    }*/

                }
                studentAnswers.Add(vec.Key, answers);
            }
        }

    }
}
