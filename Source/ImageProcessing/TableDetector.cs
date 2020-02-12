using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Class detecting table inside image after changing perspective.
    /// </summary>
    class TableDetector
    {
        /// <value> Image before table detection. </value>
        private Image<Gray, byte> image = null;

        /// <value> Area containing anwer table (before tranformation). </value>
        public Image<Gray, byte> detectedImage = null;

        /// <value> Answer table after tranformation. </value>
        public Image<Gray, byte> transformedImage = null;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputImage"> Image after changing perspective. </param>
        public TableDetector(Image<Gray, byte> inputImage)
        {
            image = new Image<Gray, byte>(inputImage.Bitmap);

            PointF size = CardConstants.Instance.getSizeOfAnswerChart();
            transformedImage = new Image<Gray, byte>(new Size((int)size.X, (int)size.Y));
            //thresholdedImage = new Image<Gray, byte>(new Size((int)size.X, (int)size.Y));

            int x = (int)(CardConstants.Instance.getUpperLeftCorner().X - CardConstants.Instance.getNumericBoxSize().X);
            int y = (int)(CardConstants.Instance.getUpperLeftCorner().Y - CardConstants.Instance.getNumericBoxSize().Y);
            int recWidth = (int)(size.X + 2 * CardConstants.Instance.getNumericBoxSize().X);
            int recHeight = (int)(size.Y + 2 * CardConstants.Instance.getNumericBoxSize().Y);

            var roi = new Rectangle(x, y, recWidth, recHeight);
            image.ROI = roi;
            detectedImage = image.Copy();
            //ImageViewer.Show(detectedImage, "image");
        }

        /// <summary>
        /// Main method detecting table inside image.
        /// </summary>
        /// <remarks>
        /// Sets result to detectedImage by ROI. 
        /// Contains <see cref="getSegmented"/>, <see cref="getTheBiggestContour"/> and <see cref="getVerticesOfTable(VectorOfPoint)"/> 
        /// </remarks>
        public void detectTable()
        {
            Image<Gray, byte> temp = new Image<Gray, byte>(detectedImage.Size);
            CvInvoke.Threshold(detectedImage, detectedImage, 0, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);

            var biggestCon = getTheBiggestContour(detectedImage);
            var points = getVerticesOfTable(biggestCon);

            if (points != null)
            {
                VectorOfPointF src = new VectorOfPointF();
                VectorOfPointF dst = new VectorOfPointF();

                //sorting based on point(0,0)
                var sortedPoints = points.OrderBy(p => Math.Sqrt(p.X * p.X + p.Y * p.Y));
                src.Push(sortedPoints.ToArray());
                dst.Push(new PointF[] { new PointF(0, 0),
                    new PointF(transformedImage.Width, 0),
                    new PointF(0, transformedImage.Height),
                    new PointF(transformedImage.Width, transformedImage.Height)
                });

                try
                {
                    Mat homographicMatrix = homographicMatrix = CvInvoke.GetPerspectiveTransform(src, dst);
                    CvInvoke.WarpPerspective(detectedImage, transformedImage, homographicMatrix, new System.Drawing.Size(transformedImage.Width, transformedImage.Height));
                }
                catch (CvException)
                {
                    throw;
                }
              
                CvInvoke.Threshold(transformedImage, transformedImage, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);
               // ImageViewer.Show(transformedImage, "image");
            } 
        }


        /// <summary>
        /// Method showing detected table.
        /// </summary>
        /// <remarks> For tests. </remarks>
        public void showTable()
        {
            ImageViewer.Show(transformedImage, "table");
        }

        #region ContourProcessing

        /// <summary>
        /// Method getting the biggest contour detected inside image.
        /// </summary>
        /// <remarks> Detecting based on contour area. <see cref="CvInvoke.ContourArea(IInputArray, bool)"/> </remarks>
        /// <returns> Vector of points describing the biggest contour. </returns>
        private VectorOfPoint getTheBiggestContour(Image<Gray,byte> copyImage)
        {
            var item = new VectorOfPoint();
            var contours = new VectorOfVectorOfPoint();
            
            CvInvoke.FindContours(copyImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

            //find the max contour
            Dictionary<int, double> dict = new Dictionary<int, double>();
            if (contours.Size > 0)
            {
                for (int i = 0; i < contours.Size; ++i)
                {
                    double area = CvInvoke.ContourArea(contours[i]);
                    dict.Add(i, area);
                }
            }
                
            item = contours[dict.OrderByDescending(v => v.Value).First().Key];  
            return item;
        }

        /// <summary>
        /// Method making approximation on contour and getting vertices as result.
        /// </summary>
        /// <remarks> If contour after approximation doesn't have four vertices, array == null. </remarks>
        /// <param name="contour"> Vector of point detected by 
        /// <see cref="CvInvoke.FindContours(IInputOutputArray, IOutputArray, IOutputArray, RetrType, ChainApproxMethod, Point)"/></param>
        /// <returns> Array of points. </returns>
        private PointF[] getVerticesOfTable(VectorOfPoint contour)
        {
            if (contour.Size == 0) return null;
            RotatedRect tablePoints = new RotatedRect();
            PointF[] points = null;
            var approxContour = new VectorOfPoint();
            
            CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
            if (approxContour.Size == 4)
            {
                tablePoints = CvInvoke.MinAreaRect(approxContour);
                points = tablePoints.GetVertices();
            }
            
            return points;
        }
        #endregion ContourProcessing
    }
}
