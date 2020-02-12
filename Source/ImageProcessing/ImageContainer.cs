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
    /// Class containing main image without any transformation.
    /// </summary>
    class ImageContainer
    {
        /// <summary>
        /// Main image, input from user.
        /// </summary>
        public Image<Gray, byte> image { get; }


        /// <summary>
        /// Constructor setting image.
        /// </summary>
        /// <param name="imageName"> Main image. </param>
        public ImageContainer(string imageName)
        {
            image = new Image<Gray, byte>(imageName);
            ImageViewer.Show(image, "mainImage");
        }

        #region imageSegmentation

        /// <summary>
        /// This static method extracts details from image, cleaning it up (remove noise and light diffrence).
        /// </summary>
        /// <param name="image"> Image to be detailed. </param>
        /// <returns> Detailed image. </returns>
        static public Image<Gray, byte> getDetailedImage(Image<Gray, byte> image)
        {
            var copyImage = image.Copy();
            MCvScalar con = new MCvScalar();
            MCvScalar blockSize = new MCvScalar();
            CvInvoke.MeanStdDev(copyImage, ref blockSize, ref con);
            blockSize.V0 += con.V0;
            if ((int)blockSize.V0 % 2 == 0) blockSize.V0 -= 1;
            Console.WriteLine("BLOCK SIZE: " + blockSize.V0 + " CON: " + con.V0);


            CvInvoke.MorphologyEx(copyImage, copyImage, MorphOp.Dilate, new Mat(), new Point(-1, -1), 1, BorderType.Default, new MCvScalar());
            CvInvoke.MedianBlur(copyImage, copyImage, (int)(blockSize.V0));
            CvInvoke.AbsDiff(image, copyImage, copyImage);
            copyImage._Not();
            CvInvoke.Normalize(copyImage, copyImage, 0, 255, NormType.MinMax, DepthType.Default);

            ImageViewer.Show(copyImage, "DETAILED IMAGE");
            return copyImage;
        }

        /// <summary>
        /// Tis method generates thresholded image using Binary Invertion and Otsu as a type of thresholding.
        /// </summary>
        /// <param name="image"> Image to be thresholded. </param>
        /// <returns> Thresholded image. </returns>
        static public Image<Gray, byte> getThresholded(Image<Gray, byte> image)
        {
            var copyImage = image.Copy();
            var tempTable = copyImage.SmoothGaussian(3);
            tempTable = copyImage - tempTable;
            copyImage = copyImage + 3 * tempTable;

            CvInvoke.Threshold(copyImage, copyImage, 100, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);
            //ImageViewer.Show(copyImage, "THRESHOLDED");
            return copyImage;
        }


        /// <summary>
        /// This method generates thresholded image using adaptive thresholding - gauss.
        /// As blockSize - a half mean value from image and constants - a half standard deviation.
        /// </summary>
        /// <param name="image"> Image to be thresholded. </param>
        /// <returns> Thresholded image. </returns>
        static public Image<Gray, byte> getThresholdedGauss(Image<Gray, byte> image)
        {
            MCvScalar con = new MCvScalar();
            MCvScalar blockSize = new MCvScalar();
            CvInvoke.MeanStdDev(image, ref blockSize, ref con);

            if ((int)blockSize.V0 % 2 == 0) blockSize.V0 -= 1;

            var copyImage = image.SmoothGaussian(3);
            copyImage = copyImage.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.GaussianC, ThresholdType.BinaryInv, (int)blockSize.V0 , new Gray((int) con.V0));

            //ImageViewer.Show(copyImage, "GAUSS");

            return copyImage;

        }

        #endregion imageSegmentation
    }


}
