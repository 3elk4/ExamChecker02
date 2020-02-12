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
    /// Class detecting index number and read each number of index.
    /// </summary>
    class IndexDetector
    {
        /// <value> Contains index number. </value>
        public int IndexNumber { get; set; }

        ///<value> List containing result of <see cref="compareSegments"/>(digits). Values are sorted from left to right. </value>
        private List<int> numbers = new List<int>();

       

        ///<value> Minimum ratio of white pixels to segment size. </value>
        private const double limit = 0.4;

        ///<value> Original image. </value>
        private Image<Gray, byte> image = null;

        ///<value> Part of image containing index number. </value>
        private Image<Gray, byte> invisibleIndexBox = null;

        ///<value> Black and white image containing filled contours of detected numbers. Same size as <see cref="invisibleIndexBox"/>. </value>
        private Image<Gray, byte> detectedNumbersImage = null;

        ///<value> Structure for detcted digits containing bouding boxes as a key and contous' point as a value. </value>
        private Dictionary<Rectangle, List<VectorOfPoint>> detectedDigits =
            new Dictionary<Rectangle, List<VectorOfPoint>>();


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks> Finds <see cref="invisibleIndexBox"/>. </remarks>
        /// <param name="image"> Original image for index detection. </param>
        public IndexDetector(Image<Gray, byte> image)
        {
            this.image = image.Clone();
        }


        public void mainProcess()
        {
            try {
                invisibleIndexBox = getInvisibleIndexBoxImage();
                ImageViewer.Show(invisibleIndexBox);
                CvInvoke.Threshold(invisibleIndexBox, invisibleIndexBox, 0, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);
            }
            catch (CvException) {
                throw;
            }

            detectedNumbersImage = new Image<Gray, byte>(invisibleIndexBox.Size);
            //ImageViewer.Show(invisibleIndexBox);

            setBoundingBoxesAndContoursDictionary();

            drawNumbers();
            try {
                numberDetection();
            }
            catch (CvException)
            {
                throw;
            }
        }


        #region settingIndex

        /// <summary>
        /// Sets digits from <see cref="numbers"/> to <see cref="IndexNumber"/> as a value.
        /// </summary>
        /// <param name="num"> List of detected digits. </param>
        public void setIndexNumber(List<int> num)
        {
            if (num == null) return;
            IndexNumber = 0;

            int l = 100000;
            num.ForEach(n => {
                IndexNumber += l * n;
                l /= 10;
            });
        }

        /// <summary>
        /// Checks if <see cref="numberDetection"/> detected all numbers inside image.
        /// </summary>
        /// <returns> True if there's no -1 value in <see cref="numbers"/></returns>
        public bool isIndexMatchingPattern()
        {
            foreach (var n in numbers)
            {
                if (n.Equals(-1)) return false;
            }
            return true;
        }

        #endregion settingIndex

        #region ImageCuttingAndDrawing

        /// <summary>
        /// Cuts out part of image with index number.
        /// </summary>
        /// <returns> Part of image with index number. </returns>
        public Image<Gray, byte> getInvisibleIndexBoxImage()
        {
            var upperLeft = CardConstants.Instance.getUpperLeftCornerOfInvisibleIndexBox();
            var bottomRight = CardConstants.Instance.getBottomRightCornerOfInvisibleIndexBox();

            var roi = new Rectangle((int)Math.Round(upperLeft.X), (int)Math.Round(upperLeft.Y), (int)Math.Round(bottomRight.X - upperLeft.X), (int)Math.Round(bottomRight.Y - upperLeft.Y));
            Image<Gray, byte> newImage;
            try {
                newImage = new Mat(image.Mat, roi).ToImage<Gray, byte>();
            }
            catch (CvException) {
                throw;
            }
            
            return newImage;
        }

        /// <summary>
        /// Draws detected digits' contours, fills them and do some morphology to makes digits more accurate. 
        /// </summary>
        public void drawNumbers()
        {
            foreach (var num in detectedDigits.Values)
            {
                num.ForEach(contour => detectedNumbersImage.DrawPolyline(contour.ToArray(), true, new Gray(255)));
            }

            //ImageViewer.Show(detectedNumbersImage, "CONTOURS");

  
            CvInvoke.MorphologyEx(detectedNumbersImage, detectedNumbersImage, MorphOp.Close, new Mat(), new Point(-1, -1), 6, BorderType.Default, new MCvScalar());
            ImageViewer.Show(detectedNumbersImage, "CLOSE");
            var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 1), new Point(-1, -1));
            CvInvoke.MorphologyEx(detectedNumbersImage, detectedNumbersImage, MorphOp.Dilate, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            var kernel2 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(1, 5), new Point(-1, -1));
            CvInvoke.MorphologyEx(detectedNumbersImage, detectedNumbersImage, MorphOp.Dilate, kernel2, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            //ImageViewer.Show(detectedNumbersImage, "DILATE");

            ImageViewer.Show(detectedNumbersImage, "FILLED");
        }

        #endregion ImageCuttingAndDrawing

        #region DigitDetection

        /// <summary>
        /// Finds six the biggest contours inside index image area.
        /// </summary>
        /// <param name="contours"> Detected contours by 
        /// <see cref="CvInvoke.FindContours(IInputOutputArray, IOutputArray, IOutputArray, RetrType, ChainApproxMethod, Point)"/> </param>
        /// <returns> List of six biggest contours. </returns>
        public List<List<VectorOfPoint>> findSixMaxContours(VectorOfVectorOfPoint contours, Dictionary<int, List<VectorOfPoint>> relatedContours)
        {
            var sixMaxContours = new List<List<VectorOfPoint>>();
            var digitsContours = new Dictionary<int, int>();

            foreach (var relCon in relatedContours)
            {
                var rect = CvInvoke.BoundingRectangle(relCon.Value[0]);
                digitsContours[relCon.Key] = rect.Size.Height;
            }

            var sortedDigitsContours = digitsContours.OrderByDescending(p => p.Value);

            for (int i = 0; i < 6; ++i)
            {
                sixMaxContours.Add(relatedContours[sortedDigitsContours.ElementAt(i).Key]);
            }

            /*
            //choose only digits for contours
            var digitsContours = new Dictionary<int, int>();
            for (int i = 0; i < contours.Size; ++i)
            {
                var rect = CvInvoke.BoundingRectangle(contours[i]);
                digitsContours[i] = rect.Size.Height;
            }

            var sortedDigitsContours = digitsContours.OrderByDescending(p => p.Value);
            for(int i = 0; i < 6; ++i)
            {
                sixMaxContours.Add(contours[sortedDigitsContours.ElementAt(i).Key]);
            }*/

            return sixMaxContours;
        }

        /// <summary>
        /// Sorts <see cref="detectedDigits"/> from left to right by bounding box upper left corner.
        /// </summary>
        /// <returns> Sorted structure of <see cref="detectedDigits"/>. </returns>
        public Dictionary<Rectangle, List<VectorOfPoint>> sortBoundingBoxes()
        {
            return detectedDigits.OrderBy(d => d.Key.Left).ToDictionary(d => d.Key, d => d.Value);
        }

        /// <summary>
        /// Creates structure of digits' bounding boxes by detecting six biggest contours. 
        /// </summary>
        /// <remarks> Structure is sorted by <see cref="sortBoundingBoxes"/> </remarks>
        public void setBoundingBoxesAndContoursDictionary()
        {
            //temporary
            //var tempImage = new Image<Gray, byte>(invisibleIndexBox.Size);
            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();

            CvInvoke.FindContours(invisibleIndexBox, contours, hierarchy, RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple);
            //tu uporządkucj kontury względem hierarchii
            Dictionary<int, List<VectorOfPoint>> relatedContours = organizeContours(contours, hierarchy);


            var listOfDigitContours = findSixMaxContours(contours, relatedContours);

            foreach (var contour in listOfDigitContours)
            {
                var rect = CvInvoke.BoundingRectangle(contour[0]);
                var digitBBSize = CardConstants.Instance.getDigitSize(rect.Height);

                var boundingBox = new Rectangle(new Point(rect.Right - (int)Math.Round(digitBBSize.X), rect.Bottom - (int)Math.Round(digitBBSize.Y)),
                    new Size((int)Math.Round(digitBBSize.X), (int)Math.Round(digitBBSize.Y)));

                detectedDigits[boundingBox] = contour;
            }

            detectedDigits = sortBoundingBoxes();

            /*
            //drawing contours

            for (int i = 0; i < 6; ++i)
            {
                tempImage.Draw(detectedDigits.ElementAt(i).Key, new Gray(255), 2);
                //tempo
                ImageViewer.Show(tempImage, "BOUNDING BOXY");
            }*/

        }



        /// <summary>
        /// This method classifies contours based on relationship between parents and children. Outer contours - parents, inner - children.
        /// </summary>
        /// <param name="contours"> Contours generated by <see cref="CvInvoke.FindContours(IInputOutputArray, IOutputArray, IOutputArray, RetrType, ChainApproxMethod, Point)"/>
        ///  and <see cref="RetrType.Ccomp"/>. </param>
        /// <param name="hierarchy"> Defines relationships between contours. </param>
        /// <returns> Structure containing number of outer contour as a key and List of all contours related as a value. </returns>
        public Dictionary<int, List<VectorOfPoint>> organizeContours(VectorOfVectorOfPoint contours, Mat hierarchy)
        {
            var contoursStructure = new Dictionary<int, List<VectorOfPoint>>();
            var data = hierarchy.GetData();

            //make parents
            for(int i = 0; i < hierarchy.Cols; ++i)
            {
                //outer contour
                if(data.GetValue(0, i, 3).Equals(-1))
                {
                    var con = contours[i];
                    var tempList = new List<VectorOfPoint> { con };

                    //make childs
                    for (int j = 0; j < hierarchy.Cols; ++j)
                    {
                        if (data.GetValue(0, j, 2).Equals(-1) && data.GetValue(0, j, 3).Equals(i))
                        {
                            var conChild = contours[j];
                            tempList.Add(conChild);
                        }
                    }
                    contoursStructure[i] = tempList;
                }
            }

            return contoursStructure;
        }

        #endregion DigitDetection

        #region DigitDetermination

        /// <summary>
        /// Cuts out area from <see cref="detectedNumbersImage"/>.
        /// </summary>
        /// <param name="bounndingBox"> Defines area to be cutted out. </param>
        /// <returns> Cutted out area. </returns>
        public Mat getNumberArea(Rectangle bounndingBox)
        {
            Mat numArea = null;
            try {
                numArea = new Mat(detectedNumbersImage.Mat, bounndingBox);
            }
            catch (CvException) {
                throw;
            }
            return numArea;
        }


        /// <summary>
        /// Process areas of numbers by each segment and defines detected numbers inside image.
        /// </summary>
        public void numberDetection()
        {
            //Console.Write("Number: ");
            foreach(var bb in detectedDigits.Keys)
            {
                Mat digitArea;
                try {
                   digitArea = getNumberArea(bb);
                }
                catch (CvException) {
                    throw;
                }

                //ImageViewer.Show(digitArea, "digit");

                Dictionary<int, Rectangle> segments = createSegments(bb);

                var temp = digitArea.Clone().ToImage<Gray, byte>();

                var numberTemp = new List<int>();
                foreach(var seg in segments)
                {
                   
                    var segArea = new Mat(digitArea, seg.Value);
                    //ImageViewer.Show(segArea, "segment");
                    var whitePix = CvInvoke.CountNonZero(segArea);
                    var ratio = (double)whitePix / segArea.Total.ToInt32();
                    

                    numberTemp.Add(Math.Round(ratio, 1) >= Math.Round(limit, 1) ? 1 : 0);
                    //Console.WriteLine("RATIO: " + Math.Round(ratio, 1));
                    //Console.WriteLine("Is On: " + ((double)whitePix / segArea.Total.ToInt32() >= limit ? 1 : 0) + ", " + (double)whitePix / segArea.Total.ToInt32());
                }

                var num = compareSegments(numberTemp);
                numbers.Add(num);
                Console.WriteLine("Number: "+ num);
            }

            if (isIndexMatchingPattern())
            {
                setIndexNumber(numbers);
            }
           
        }

        #endregion DigitDetermination

        #region Segments

        /// <summary>
        /// Compares segments with key (<see cref="Digits.digits"/>.
        /// </summary>
        /// <param name="segmentNumber"> List of defined segments. </param>
        /// <returns> Detected number or -1 when sequence is not defined. </returns>
        public int compareSegments(List<int> segmentedNumber)
        {
            foreach (var dig in Digits.digits)
            {
                if (dig.Value.SequenceEqual(segmentedNumber))
                    return dig.Key;
            }

            return -1;
        }


        /// <summary>
        /// Creates segments for each bounding box.
        /// </summary>
        /// <param name="boundingBox"> Defined bounding box for number. </param>
        /// <remarks> Segments are shifted from edges by offset. </remarks>
        /// <returns> Structure of segment's number as a key and segment's area as a value. </returns>
        public Dictionary<int, Rectangle> createSegments(Rectangle boundingBox)
        {
            var segments = new Dictionary<int, Rectangle>();

            int offset = 2;
            int thickness = (int)Math.Round(0.12F * boundingBox.Height);
            Size hor = new Size(boundingBox.Width - offset * 2 - 2 * thickness, thickness);
            Size ver = new Size(thickness, (int)Math.Round(boundingBox.Height * 0.5 - 2*offset - 1.5 * thickness));

            segments[1] = new Rectangle(new Point(0 + offset + thickness, 0 + offset), hor);
            segments[2] = new Rectangle(new Point(boundingBox.Width - ver.Width - offset, 0 + offset + thickness), ver);
            segments[3] = new Rectangle(new Point(boundingBox.Width - ver.Width - offset, (int)Math.Round(boundingBox.Height / 2 + offset + 0.5 * thickness)), ver);
            segments[4] = new Rectangle(new Point(0 + offset + thickness, boundingBox.Height - hor.Height - offset), hor);
            segments[5] = new Rectangle(new Point(0 + offset, (int)Math.Round(boundingBox.Height / 2 + offset + 0.5 * thickness)), ver);
            segments[6] = new Rectangle(new Point(0 + offset, 0 + offset + thickness), ver);
            segments[7] = new Rectangle(new Point(0 + offset + thickness, boundingBox.Height/2 - hor.Height/2), hor);

            /*var tempImage = new Image<Gray, byte>(boundingBox.Size);
            foreach(var seg in segments)
            {
                tempImage.Draw(seg.Value, new Gray(255), 1);
                ImageViewer.Show(tempImage, "segments");
            }*/

            return segments;
        }

        #endregion Segments
    }
}
