using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Aruco;
using Emgu.CV.Util;
using Emgu.CV.Structure;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Class detecting aruco markers inside image.
    /// </summary>
    /// <remarks> Detected markers allow change perspective of the image. </remarks>
    class ArucoDetector
    {
        /// <value> Aruco markers existing in the dictionary. </value>
        private Dictionary arucoMarkerDictionary;

        /// <value> Vector of detected outer markers' corners. </value>
        private PointF[] src;

        /// <value> Vector of new corners for tranformed image. </value>
        private PointF[] dst;

        /// <value> Detected aruco markers structure. Value of marker as key and vector of makers's points as value. </value>
        private SortedDictionary<int, PointF[]> idsAndCorners;

        /// <value> Size of image [px]. </value>
        private PointF imageSize;

        /// <value> The smallest marker's value. </value>
        private int minMarker;

        /// <value> Image before changing perspective. </value>
        public Image<Gray, byte> inputImage{ get; }

        /// <value> Image after changing perspective. </value>
        public Image<Gray, byte> outputImage { get; }

        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <remarks> Contains initializing dictionary for aruco markers and size for tranformed image. </remarks>
        /// <param name="image"> Basic image with aruco markers. </param>
        public ArucoDetector(Image<Gray, byte> image) {
           arucoMarkerDictionary = new Dictionary(Dictionary.PredefinedDictionaryName.Dict4X4_50);
            inputImage = new Image<Gray, byte>(image.Bitmap);

            imageSize = CardConstants.Instance.getImageSize(500);
            
            outputImage = new Image<Gray, byte>((int)Math.Round(imageSize.X), (int)Math.Round(imageSize.Y));

            src = new PointF[4];
            dst = new PointF[4];
            idsAndCorners = new SortedDictionary<int, PointF[]>();
        }


        /// <summary>
        /// Method detecting values and position of aruco markers inside image. 
        /// </summary>
        /// <remarks> Puts data into aruco markers structure and vectors of corners. </remarks>
        /// <param name="image"> Basic image with aruco markers. </param>
        public void ArucoMarkersPoints(Image <Gray, byte> image)
        {
            
            using (VectorOfVectorOfPointF corners = new VectorOfVectorOfPointF())
            {
                using (VectorOfInt ids = new VectorOfInt())
                {
                    ArucoInvoke.DetectMarkers(image, arucoMarkerDictionary, corners, ids, DetectorParameters.GetDefault());
                    
                    //ArucoInvoke.DrawDetectedMarkers(inputImage, corners, ids, new MCvScalar(0, 0, 255));
                    /* if(ids.Size > 0)
                     {
                         ArucoInvoke.EstimatePoseSingleMarkers();
                     }*/

                    try
                    {
                        SetMarkerDictionary(ids, corners);
                        

                        
                        src = new PointF[] { new PointF(idsAndCorners[minMarker][0].X, idsAndCorners[minMarker][0].Y),
                                            new PointF(idsAndCorners[minMarker+1][1].X, idsAndCorners[minMarker+1][1].Y),
                                            new PointF(idsAndCorners[minMarker+2][2].X, idsAndCorners[minMarker+2][2].Y),
                                            new PointF(idsAndCorners[minMarker+3][3].X, idsAndCorners[minMarker+3][3].Y) };
                    }
                    catch(ArgumentException)
                    {
                        throw;
                    }
                   
                }
            }


            dst = new PointF[] { new Point(0, 0), new PointF(imageSize.X, 0), new PointF(imageSize.X, imageSize.Y), new PointF(0, imageSize.Y) };
        }

      
        /// <summary>
        /// Method setting values of detected aruco markers to structure.  
        /// </summary>
        /// <exception cref="ArgumentException"> When quntity of markers is not enough. </exception>
        /// <param name="ids"> Markers' values. </param>
        /// <param name="corners"> Corners of each detected marker. </param>
        public void SetMarkerDictionary(VectorOfInt ids, VectorOfVectorOfPointF corners)
        {
            if (ids == null || ids.Size < 4)
            {
                throw new ArgumentException("I haven't found aruco markers.");
            }

            idsAndCorners.Clear();
            var i = 0;
            minMarker = ids.ToArray().Min();
            foreach (var key in ids.ToArray())
                idsAndCorners.Add(key, corners[i++].ToArray());
        }

        /// <summary>
        /// Method changing perspective of image based on aruco markers' positions.
        /// </summary>
        public void ChangePerspective()
        {
            Mat homographicMatrix = null;
            try {
                homographicMatrix = CvInvoke.GetPerspectiveTransform(src, dst);
               
            }
            catch (CvException)
            {
                 throw;
            }
            CvInvoke.WarpPerspective(inputImage, outputImage, homographicMatrix, new System.Drawing.Size(outputImage.Width, outputImage.Height));
        }

    }
}
