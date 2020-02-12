using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Singleton class containing basic measurements of image's elements. 
    /// </summary>
    class CardConstants
    {
        private static CardConstants instance = null;

        /// <summary>
        /// Instance of class.
        /// </summary>
        public static CardConstants Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new CardConstants();
                }
                return instance;
            }
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        /// <remarks> Initializes measurements in centimeters. </remarks>
        private CardConstants()
        {
            cardSize = new PointF(16.0F, 24.7F);

            //for perfect model
            upperLeftCorner = new PointF(11.7F, 3.5F);
            bottomRightCorner = new PointF(14.15F, 20.5F);
            numericBox = new PointF(0.775F, 0.655F);
            answerBox = new PointF(0.63F, 0.7F); //y = 0.7??
            squareOffset = new PointF(0.175F, 0.155F);
            squareBoxSize = new PointF(0.3F, 0.3F);//


            //index detection
            indexBoxUpperLeftCorner = new PointF(0, 10.5F);
            indexBoxBottomRightCorner = new PointF(10, 13.5F);
            digitSize = new PointF(0.9F, 2.0F);
        }


        #region SET&GET
        public void setQuantityOfChart(int questionQuantity, int answrPerQuestion)
        {
            this.questionQuantity = questionQuantity;
            this.answerPerQuestionQuantity = answrPerQuestion;
        }

        public int getQuestionQuantity()
        {
            return this.questionQuantity;
        }

        public int getAnswerPerQuestionQuantity()
        {
            return this.answerPerQuestionQuantity;
        }
        #endregion SET&GET

        #region actualConstants
        /// <value> Size between aruco markers [cm]. </value>
        private PointF cardSize;



        /// <value> Quantity of chart's question. </value>
        private int questionQuantity;

        /// <value> Quantity of chart's answer per question. </value>
        private int answerPerQuestionQuantity;

        /// <value> Upper left corner of answer chart [cm]. </value>
        private PointF upperLeftCorner;

        /// <value> Bottom right corner of answer chart. [cm]. </value>
        private PointF bottomRightCorner;

        /// <value> Size of "numeric" box of answer chart [cm]. </value>
        private PointF numericBox;

        ///<value> Size of answer box of answer chart. [cm] </value>
        private PointF answerBox;

        ///<value> Offset square of answer of left lines of answer table. [cm] </value>
        private PointF squareOffset;

        /// <value> Size of answer box. [cm] </value>
        private PointF squareBoxSize;



        /// <value> Upper left corner of invisible box for index [cm]. </value>
        private PointF indexBoxUpperLeftCorner;

        /// <value> Bottom right corner of invisible box for index [cm]. </value>
        private PointF indexBoxBottomRightCorner;

        ///<value> Size of one digit number [cm]. </value>
        private PointF digitSize;


        #endregion actualConstants

        #region PixelMeasures

        /// <value> Size between aruco markers/ size of image [px]. </value>
        private PointF imageSize = new PointF();



        /// <value>  Upper left corner of answer table [px]. </value>
        private PointF upperLeftPoint = new PointF();

        /// <value> Bottom right corner of answer table [px]. </value>
        private PointF bottomRightPoint = new PointF();

        /// <value> Size of "numeric" box of answer chart [px]. </value>
        private PointF numericBoxPX = new PointF();

        ///<value> Size of answer box of answer chart. [px] </value>
        private PointF answerBoxPX = new PointF();

        ///<value> Offset square of answer of left lines of answer table. [px] </value>
        private PointF squareOffsetPX = new PointF();

        /// <value> Size of answer box. [px] </value>
        private PointF squareBoxSizePX = new PointF();



        /// <value> Upper left corner of invisible box for index [px]. </value>
        private PointF indexBoxUpperLeftPoint = new PointF();

        ///<value> Bottom right corner of invisible box for index [px]. </value>
        private PointF indexBoxBottomRightPoint = new PointF();

        ///<value> Size of one digit number [px]. </value>
        private PointF digitSizePX = new PointF();

        #endregion PixelMeasures

        #region Functions

        /// <summary>
        /// Returns size of image in pixels.
        /// </summary>
        /// <remarks> Based on ratio Y[cm]/X[cm] = y[px]/x[px]. </remarks>
        /// <param name="width"> Width of image in pixels. </param>
        /// <returns> Size in pixels. </returns>
        public PointF getImageSize(float width)
        {
            if (imageSize.IsEmpty)
            {
                imageSize.X = width;
                imageSize.Y = (cardSize.Y / cardSize.X) * width;
            }
            return imageSize;
        }


        /// <summary>
        /// Returns position of upper left corner of table in pixels.
        /// </summary>
        /// <remarks> Based on ratio a[cm]/A[cm] = b[px]/B[px]. </remarks>
        /// <returns> Position of upper left corner of table in pixels. </returns>
        public PointF getUpperLeftCorner()
        {
            if (upperLeftPoint.IsEmpty && !imageSize.IsEmpty)
            {
                upperLeftPoint.X = (upperLeftCorner.X * imageSize.X) / cardSize.X;
                upperLeftPoint.Y = (upperLeftCorner.Y * imageSize.Y) / cardSize.Y;
            }
            return upperLeftPoint;
        }


        /// <summary>
        /// Returns position of bottom right corner of table in pixels.
        /// </summary>
        /// <remarks> Based on ratio a[cm]/A[cm] = b[px]/B[px]. </remarks>
        /// <returns> Position of bottom right corner of table in pixels. </returns>
        public PointF getBottomRightCorner()
        {
            if (bottomRightPoint.IsEmpty && !imageSize.IsEmpty)
            {
                bottomRightPoint.X = (bottomRightCorner.X * imageSize.X) / cardSize.X;
                bottomRightPoint.Y = (bottomRightCorner.Y * imageSize.Y) / cardSize.Y;
            }
            return bottomRightPoint;
        }

        /// <summary>
        /// Method generates size of answer chart or charts (as a one object).
        /// </summary>
        /// <returns> Size of answer chart [px]. </returns>
        public PointF getSizeOfAnswerChart()
        {
            PointF size = new PointF();
            if (!imageSize.IsEmpty)
            {
                size.X = ((bottomRightCorner.X - upperLeftCorner.X) * imageSize.X) / cardSize.X;
                size.Y = ((bottomRightCorner.Y - upperLeftCorner.Y) * imageSize.Y) / cardSize.Y;
            }
            return size;
        }




        /// <summary>
        /// Returns size of "numeric" rectangle of table in pixels.
        /// </summary>
        /// <remarks> Based on ratio a[cm]/A[cm] = b[px]/B[px]. </remarks>
        /// <returns> Size of "numeric" rectangle in pixels. </returns>
        public PointF getNumericBoxSize()
        {
            if (numericBoxPX.IsEmpty && !imageSize.IsEmpty)
            {
                numericBoxPX.X = (numericBox.X * imageSize.X) / cardSize.X;
                numericBoxPX.Y = (numericBox.Y * imageSize.Y) / cardSize.Y;
            }
            return numericBoxPX;
        }

        /// <summary>
        /// Returns size of answer rectangle of table in pixels.
        /// </summary>
        /// <remarks> Based on ratio a[cm]/A[cm] = b[px]/B[px]. </remarks>
        /// <returns> Size of answer rectangle in pixels. </returns>
        public PointF getAnswerBoxSize()
        {
            if (answerBoxPX.IsEmpty && !imageSize.IsEmpty)
            {
                answerBoxPX.X = (answerBox.X * imageSize.X) / cardSize.X;
                answerBoxPX.Y = (answerBox.Y * imageSize.Y) / cardSize.Y;
            }
            return answerBoxPX;
        }

        /// <summary>
        /// Returns offset of square answer box in px.
        /// </summary>
        /// <returns> Offset of square answer box in px. </returns>
        public PointF getSquareOffset()
        {
            if (squareOffsetPX.IsEmpty && !imageSize.IsEmpty)
            {
                squareOffsetPX.X = (squareOffset.X * imageSize.X) / cardSize.X;
                squareOffsetPX.Y = (squareOffset.Y * imageSize.Y) / cardSize.Y;
            }
            return squareOffsetPX;
        }

        /// <summary>
        /// Returns size of square box in pixels.
        /// </summary>
        /// <returns> Size of square box in pixels. </returns>
        public PointF getSquareBoxSize()
        {
            if (squareBoxSizePX.IsEmpty && !imageSize.IsEmpty)
            {
                squareBoxSizePX.X = (squareBoxSize.X * imageSize.X) / cardSize.X;
                squareBoxSizePX.Y = (squareBoxSize.Y * imageSize.Y) / cardSize.Y;
            }
            return squareBoxSizePX;
        }




        /// <summary>
        /// Calculates position for index box.
        /// </summary>
        /// <returns> Position of upper left corner of index box in pixels. </returns>
        public PointF getUpperLeftCornerOfInvisibleIndexBox()
        {
            if (indexBoxUpperLeftPoint.IsEmpty && !imageSize.IsEmpty)
            {
                indexBoxUpperLeftPoint.X = (indexBoxUpperLeftCorner.X * imageSize.X) / cardSize.X;
                indexBoxUpperLeftPoint.Y = (indexBoxUpperLeftCorner.Y * imageSize.Y) / cardSize.Y;
            }
            return indexBoxUpperLeftPoint;
        }

        /// <summary>
        /// Calculates position for index box.
        /// </summary>
        /// <returns> Position of bottom right corner of index box in pixels. </returns>
        public PointF getBottomRightCornerOfInvisibleIndexBox()
        {
            if (indexBoxBottomRightPoint.IsEmpty && !imageSize.IsEmpty)
            {
                indexBoxBottomRightPoint.X = (indexBoxBottomRightCorner.X * imageSize.X) / cardSize.X;
                indexBoxBottomRightPoint.Y = (indexBoxBottomRightCorner.Y * imageSize.Y) / cardSize.Y;
            }
            return indexBoxBottomRightPoint;
        }



        /// <summary>
        /// Created size for bounding box based on indexBox size.
        /// </summary>
        /// <param name="heightBox"> Height of detected digit's contour (from <see cref="CvInvoke.BoundingRectangle()"/>). </param>
        /// <returns> Size for digit's area. </returns>
        public PointF getDigitSize(float heightBox)
        {
            if (digitSizePX.IsEmpty && !indexBoxBottomRightPoint.IsEmpty && !indexBoxUpperLeftPoint.IsEmpty)
            {
                //SIZE OF INVISIBLE INDEX BOX 
                var invisibleBoxSize = new PointF(indexBoxBottomRightPoint.X - indexBoxUpperLeftPoint.X, indexBoxBottomRightPoint.Y - indexBoxUpperLeftPoint.Y); 

                //SETTING SIZE FOR DIGIT BB
                digitSizePX.Y = heightBox;
                digitSizePX.X = (invisibleBoxSize.X / (invisibleBoxSize.Y * 6)) * heightBox;
            }
            return digitSizePX;
        }
        #endregion Functions

    }
}
