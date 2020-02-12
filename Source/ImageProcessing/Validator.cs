using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// This enum contains types of validation for image.
    /// </summary>
    /// <remarks>
    /// Types: clear, overexposed, underexposed, indistinct.
    /// </remarks>
    public enum ValidationType
    {
        CLEAR, OVEREXPOSED, UNDEREXPOSED, INDISTINCT 
    }

    #region ShowValidations

    /// <summary>
    /// This class shows state of image after validation.
    /// </summary>
    /// <remarks>
    /// Contains instance of <c>Validator</c> and method checking state of image.
    /// </remarks>
    public sealed class ShowValidations
    {
        /// <value> Image path. </value>
        private String imagePath = null;

        /// <value> Instance of class performing validation on image. </value>
        private Validator validator = null;

        /// <value> Contains validation type and connected instance of exception. </value>
        private Dictionary<ValidationType, Exception> validateExceptions;

        /// <value> Contains types of validation detected in image. </value>
        private List<ValidationType> imageValidations;
        
        /// <value> Gets information about image after validation. </value>
        public Boolean IsClear { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Initialize validateExceptions and creates new instance of Validator.
        /// </remarks>
        /// <param name="imagePath"> Path to image. </param>
        public ShowValidations(String imagePath)
        {
            validateExceptions = new Dictionary<ValidationType, Exception>();
            imageValidations = new List<ValidationType>();

            validateExceptions.Add(ValidationType.OVEREXPOSED, new OverexposedException());
            validateExceptions.Add(ValidationType.UNDEREXPOSED, new UnderexposedException());
            validateExceptions.Add(ValidationType.INDISTINCT, new SharpenessException());

            this.imagePath = imagePath;
            validator = new Validator(this.imagePath);
            IsClear = true;
        }



        /// <summary>
        /// Method checking quality of the image.
        /// </summary>
        /// <remarks>
        /// Save results to validation-type list and sets flag 
        /// </remarks>
        public void check()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (validator == null) {
                //probably imagePath == null
                throw new NullReferenceException();
            }

            imageValidations.Add(validator.checkBrightnessOfImage());
            imageValidations.Add(validator.checkSharpenessOfImage());

            imageValidations.ForEach(type => { if (!type.Equals(ValidationType.CLEAR)) IsClear = false; });

            watch.Stop();
            Console.WriteLine("Time: " + watch.ElapsedMilliseconds + " ms.");
        }

        /// <summary>
        /// Method showing types of validation occurred during checking image.
        /// </summary>
        /// <remarks> Skips when image is clear. </remarks>
        public void show()
        {
            imageValidations.ForEach(type => {
                if (!type.Equals(ValidationType.CLEAR))
                    Console.WriteLine("Validation: {0}", validateExceptions[type].ToString());
            });
        }
    }

    #endregion ShowValidations

    #region Validator

    /// <summary>
    /// This class contains methods that check quality of image and determine its usability.
    /// </summary>
    /// <remarks>
    /// This class define if image is overexposed, underexposed or indistinct.
    /// </remarks>
    public class Validator
    {
        /// <value> Main image to validate. </value>
        private Image<Gray, byte> imageBeforeValidation;

        /// <summary>
        /// Constructor. Initialize image to validation.
        /// </summary>
        /// <param name="image"> Image path. </param>
        public Validator(String image)
        {
            imageBeforeValidation = new Image<Gray, byte>(image);
        }

        /// <summary>
        /// Method checking image quality based on brightness.
        /// </summary>
        /// <remarks>
        /// Uses histogram divided into 5 parts to define quality of image's brightness.
        /// </remarks>
        /// <returns> If image is too bright -> overexposed, too dark -> underexposed, enough bright -> clear. </returns>
        public ValidationType checkBrightnessOfImage()
        {
            int parNum = 5, maxHist = 256, index = 0, maxIndex = 0, aveIndex = 2;
            float maxValue = 0;
            int diffPart = maxHist / parNum;
            float[] parts = new float[parNum];
            float[] hist = new float[maxHist];

            DenseHistogram getHist = new DenseHistogram(255, new RangeF(0, 255));
            getHist.Calculate(new Image<Gray, byte>[] { imageBeforeValidation }, true, null);
            getHist.CopyTo(hist);
           
            for (int i = 0; i < maxHist; ++i) {
                if(i == diffPart && i != (maxHist - 1)){
                    diffPart += (maxHist / parNum);
                    index++;
                }
                
                parts[index] += hist[i];
            }

            maxValue = parts[0];
            for(int i = 0; i < parNum; ++i) {
                if(parts[i] > maxValue) {
                    maxValue = parts[i];
                    maxIndex = i;
                }
                Console.WriteLine(parts[i]);
            }

            //only third and fourth part of histogram are proper for image
            return (maxIndex == aveIndex || maxIndex == aveIndex + 1) ? ValidationType.CLEAR : (maxIndex < aveIndex ? ValidationType.UNDEREXPOSED : ValidationType.OVEREXPOSED);
        }

        //check if image is INDISTINCT
        /// <summary>
        /// Method checking image quality based on clarity.
        /// </summary>
        /// <remarks>
        /// Uses variance of the laplaced image and otsu-thresholding value to define quality of image's clarity.
        /// </remarks>
        /// <returns> If image is blured -> indistinct, else -> clear. </returns>
        public ValidationType checkSharpenessOfImage()
        {            
            Image<Gray, float> laplaced = imageBeforeValidation.Laplace(1);
            //ImageViewer.Show(laplaced, "Laplaced");
            Gray avg = new Gray();
            MCvScalar sdv = new MCvScalar();
            laplaced.AvgSdv(out avg, out sdv);
            double otsuThresholdValue = CvInvoke.Threshold(imageBeforeValidation, laplaced, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);

            return (sdv.V0*sdv.V0) < otsuThresholdValue ? ValidationType.INDISTINCT : ValidationType.CLEAR;
        }

    }

    #endregion Validator

    #region ImageTypeExceptions

    /// <summary>
    /// Exception class for indistinct image.
    /// </summary>
    class SharpenessException : Exception
    {
        public SharpenessException()
        {
            //Console.WriteLine("SharpenessException here!");
        }

        public override string ToString()
        {
            return base.ToString() + " Your image is indistinct!";
        }
    }


    /// <summary>
    /// Exception class for overexposed image.
    /// </summary>
    class OverexposedException : Exception
    {
        public OverexposedException()
        {
           // Console.WriteLine("OverexposedException here!");
        }

        public override string ToString()
        {
            return base.ToString() + " Your image is too bright!";
        }
    }


    /// <summary>
    /// Exception class for underexposed image.
    /// </summary>
    class UnderexposedException : Exception
    {
        public UnderexposedException()
        {
           // Console.WriteLine("UnderexposedException here!");
        }

        public override string ToString()
        {
            return base.ToString() + " Your image is too dark!";
        }
    }

    #endregion ImageTypeExceptions

}
