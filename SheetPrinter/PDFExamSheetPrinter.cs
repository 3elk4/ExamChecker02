using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamChecker.SheetGenerator.DataModels.ExamSheet;
using PdfSharp.Pdf;
using Markdig.Extensions;
using PdfSharp.Drawing;
using System.Diagnostics;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp;
using ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard;

namespace ExamChecker.SheetGenerator.SheetPrinter
{


    /// <summary> Klasa odpowiedzialna za wydruk arkusza egzaminacyjnego w formie PDF. </summary>
    class PDFExamSheetPrinter : ExamSheetPrinter
    {
        /// <summary> Nazwa pliku z rozszerzeniem PDF. </summary>
        private String PDFname;

        /// <summary> Okreslają wygląd pdf, rozmiar czcionki itp. </summary>
        private string css = "p, div, li { page-break-inside: avoid; font-size: 11px; } \n" +
                                "ol ol { list-style-type: lower-alpha; } \n" +
                                "code {  } ";

        public PDFExamSheetPrinter(String PDFname)
        {
            this.PDFname = PDFname;
        }

        /// <summary> Wydruk jednego arkuza egzaminacyjnego do pliku PDF. </summary>
        public override void Print(Sheet sheet)
        {
            var cssData = PdfGenerator.ParseStyleSheet(css);
            var htmlData = HTMLExamSheetPrinter.RawHTMLText(sheet);
            PdfDocument pdf = PdfGenerator.GeneratePdf(htmlData, PageSize.A4, 20, cssData);

            var answerSheet = new AnswerCardGenerator(pdf);
            pdf = answerSheet.GenerateAnswerCard(sheet);

            if (pdf.PageCount % 2 != 0) pdf.AddPage();
            pdf.Save(PDFname);

            Console.WriteLine("DONE");
        }

        /// <summary> Wydruk wielu arkuszy egzaminacyjnych do jednego pliku PDF. </summary>
        public override void PrintAll(List<Sheet> sheets)
        {
            PdfDocument pdf = new PdfDocument();
            var cssData = PdfGenerator.ParseStyleSheet(css);

            sheets.ForEach(sheet => {
                var htmlData = HTMLExamSheetPrinter.RawHTMLText(sheet);
                
                PdfGenerator.AddPdfPages(pdf, htmlData, PageSize.A4, 20, cssData);
                var answerSheet = new AnswerCardGenerator(pdf);
                pdf = answerSheet.GenerateAnswerCard(sheet);
                if (pdf.PageCount % 2 != 0) pdf.AddPage();
            });
            pdf.Save(PDFname);

            Console.WriteLine("DONE");
        }
    }
}
