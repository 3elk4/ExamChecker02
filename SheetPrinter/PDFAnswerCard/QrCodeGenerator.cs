using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode;

namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    class QrCodeGenerator
    {
        private readonly string _id;

        public QrCodeGenerator(string id)
        {
            _id = id;
        }

        public Bitmap GenerateQrCode()
        {
            var qrCodeWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 120,
                    Width = 120,
                    Margin = 0
                }
            };

            var pixelData = qrCodeWriter.Write(_id);
            using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            using (var ms = new MemoryStream())
            {
                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image    
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }
                // save to stream as PNG    
                //bitmap.Save("qrcode" + Guid.NewGuid() + ".png", System.Drawing.Imaging.ImageFormat.Png);

                return new Bitmap(bitmap);
            }

        }
    }
}
