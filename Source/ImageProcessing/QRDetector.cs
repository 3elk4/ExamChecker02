using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace ExamChecker.Source.ImageProcessing
{
    /// <summary>
    /// Class detecting QR code inside image.
    /// </summary>
    class QRDetector
    {
        ///<value> Instance of class decoding QR code inside image. </value>
        BarcodeReader barcodeReader = null;

        /// <value> Basic image. </value>
        Image<Gray, byte> image = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="img"> Basic image. </param>
        public QRDetector(Image<Gray, byte> img)
        {
            this.image = img;
            barcodeReader = new BarcodeReader { AutoRotate = true, TryInverted = true };
        }


        /// <summary>
        /// Method finding and decoding QR code inside image.
        /// </summary>
        /// <exception cref="NullReferenceException"> If there is no QR code inside image or detected barcode is not QR code. </exception>
        /// <returns> Value detected from QR code. </returns>
        public String findAndDecode()
        {
            var result = barcodeReader.Decode(image.ToBitmap());
            if (result != null)
            {
                if (result.BarcodeFormat.Equals(BarcodeFormat.QR_CODE))
                {
                    return result.Text.ToString();
                }
                else
                {
                    throw new NullReferenceException();
                }
            }
            else
            {
                throw new NullReferenceException();
            }

        }
    }
}
