using System;

using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.UI;
using Emgu.CV.Util;
using Emgu.CV.CvEnum;
using System.Linq;
using System.Collections.Generic;

namespace ExamChecker.Source.ImageProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            String imageName = "tests/fulltest9.jpg";
            CardConstants.Instance.setQuantityOfChart(25, 4);

            #region Validation
            ImageContainer imageContainer = new ImageContainer(imageName);
            ShowValidations showValidations = new ShowValidations(imageName);

            showValidations.check();
            showValidations.show();

            if (!showValidations.IsClear)
            {
                Console.WriteLine("Defective image.");
                Console.ReadKey();
                return;
            }

            #endregion Validation

            #region QRCode
            //qrcode detection
            QRDetector detector = new QRDetector(imageContainer.image);
            String SheetID = "";
            try {
                SheetID = detector.findAndDecode();
            }
            catch (NullReferenceException) {
                throw;
            }
            #endregion QRCode

            #region Aruco
            ArucoDetector arucoDetector = new ArucoDetector(imageContainer.image);
            

            //change percpective
            try {
                arucoDetector.ArucoMarkersPoints(imageContainer.image);
                arucoDetector.ChangePerspective();
            }
            catch(ArgumentException) {
                throw;
            }
            catch (CvException) {
                throw;
            }

            //ImageViewer.Show(arucoDetector.outputImage, "Changed Perspective");
            #endregion Aruco

            //removing shadow
            var detailedImage = ImageContainer.getDetailedImage(arucoDetector.outputImage);

            #region AnswerTable
            TableDetector tableDetector = new TableDetector(detailedImage);
            try
            {
                tableDetector.detectTable();
            }
            catch (CvException)
            {
                throw;
            }
           
            AnswerChecker answerChecker = new AnswerChecker(tableDetector.transformedImage);
            List<StudentInfo.Question> answers;
            try
            {
                answerChecker.checkAnswers();
            }
            catch (Exception)
            {
                throw;
            }
            answerChecker.showStudentAnswers();
            answers = answerChecker.ConvertToStudentExamData();

            #endregion AnswerTable

            #region Index
            String studentID = "";
            IndexDetector indexDetector = new IndexDetector(detailedImage);
             try {
                 indexDetector.mainProcess();
                 studentID = indexDetector.IndexNumber.ToString();
             }
             catch (CvException) {
                 throw;
             }
            #endregion Index

            StudentInfo.StudentExamData studentExamData = new StudentInfo.StudentExamData(SheetID, studentID, answers);

            Console.WriteLine();
            Console.WriteLine("SHEET ID: " + studentExamData.SheetID);
            Console.WriteLine("STUDENT ID: " + studentExamData.StudentID);
            Console.WriteLine("QUESTIONS AND ANSWERS:");
            foreach (var kv in studentExamData.Answers)
            {
                Console.WriteLine("QUESTION NUMBER: " + kv.QuestionNumber);
                kv.Answers.ForEach(answ => Console.WriteLine("    ANSWER ID: " + answ.AnswerNumber + " MARKED: " + answ.Marked));
            }

            Console.ReadKey();
        }

    }
}


/*
TableManager tableManager = new TableManager(merged);
try {
    tableManager.mainProcess();
}
catch (CvException) {
    throw;
}
catch (NullReferenceException) {
    throw;
}
catch(InvalidNumberException) {
    throw;
}
catch (IndexOutOfRangeException) {
    throw;
}*/
