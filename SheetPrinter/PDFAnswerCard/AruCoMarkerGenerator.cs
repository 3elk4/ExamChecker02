using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Util;
using Aruco = Emgu.CV.Aruco;

namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    class AruCoMarkerGenerator
    {
        private readonly int _startIndex;
        public AruCoMarkerGenerator(int index)
        {
            _startIndex = index;
        }
        public OutputArray[] GenerateMarkers()
        {
            IOutputArray[] output = new IOutputArray[4];
            for (int i = 0; i < 4; ++i)
            {
                output[i] = new Mat();
            }

            for (int i = 0; i < 4; ++i)
            {
                Console.WriteLine(output[i].GetOutputArray().GetSize().ToString());
                Aruco.ArucoInvoke.DrawMarker(new Aruco.Dictionary(Aruco.Dictionary.PredefinedDictionaryName.Dict4X4_50), _startIndex + i, 100, output[i], 1);
            }
            List<OutputArray> generatedArrays = new List<OutputArray>();
            foreach (var array in output)
            {
                generatedArrays.Add(array.GetOutputArray());
            }

            /*foreach (var arr in generatedArrays)
            {
                arr.GetMat().Save("marker" + Guid.NewGuid() + ".png");
            }*/
            return generatedArrays.ToArray();
        }
    }
}
