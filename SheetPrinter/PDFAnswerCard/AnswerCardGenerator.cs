using ExamChecker.SheetGenerator.DataModels.ExamSheet;
using Emgu.CV;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    /// <summary> Klasa odpowiadająca za wydenerowanie karty odpowiedzi. </summary>
    class AnswerCardGenerator
    {
        /// <summary> Referencja do dokumentu pdf. </summary>
        private PdfDocument document;

        //markers
        /// <summary> Bitmapa QRCodu na podstawie ID egzaminu. </summary>
        private Bitmap _qrcode;
        /// <summary> Markery AruCo. </summary>
        private OutputArray[] _arrays_aruco;

        //index
        /// <summary> Wygenerowany szablon na wpisanie numeru albumu. </summary>
        private TextFrame _index_table;
        /// <summary> Zasady dotyczące wpisywania numeru albumu. </summary>
        private TextFrame _index_table_rules;

        //answer table
        /// <summary> Tabela na odpowiedzi. </summary>
        private Table _answer_table;
        /// <summary> Zasady wypełniania tabeli na odpowiedzi. </summary>
        private TextFrame _answer_table_rules;

        //student info table
        /// <summary> Tabela na podstawowe dane studenta. </summary>
        private Table _student_info_table;

        public AnswerCardGenerator(PdfDocument document)
        {
            this.document = document;
        }

        /// <summary> Generowanie karty odpowiedzi i brudnopisu. </summary>
        public PdfDocument GenerateAnswerCard(Sheet sheet)
        {
            GenerateMarkers(sheet.SheetID.ToString());
            GenerateIndexTable();
            GenerateAnswerTable(100,5); //sheet.Questions.Length, MaxAnswerNumber(sheet)
            GenerateStudentInfoTable(sheet.AvailablePoints);

            CreateAnswerSheetPage(sheet.SheetID.ToString(), sheet.Subject.Name, sheet.Comment);
            CreateRoughPage(sheet.Subject.Name + " - Brudnopis");

            return this.document;
        }

        /// <summary> Maksymalna liczba odpowiedzi jaka może pojawić się w pytaniach. </summary>
        private int MaxAnswerNumber(Sheet sheet)
        {
            int max = 0;
            foreach (var question in sheet.Questions)
            {
                if (question is SingleChoiceQuestion _sq) {
                    if (_sq.Options.Length > max) max = _sq.Options.Length;
                }
                else if (question is MultipleChoiceQuestion _mq) {
                    if (_mq.Options.Length > max) max = _mq.Options.Length;
                }   
            }
            return max;
        }

        #region MARKERS GENERATION & RENDERING
        /// <summary> Generowanie markerów. </summary>
        private void GenerateMarkers(string sheet_id)
        {
            AruCoMarkerGenerator aruco_generator = new AruCoMarkerGenerator(1);
            _arrays_aruco = aruco_generator.GenerateMarkers();

            QrCodeGenerator qrCodeGenerator = new QrCodeGenerator(sheet_id);
            _qrcode = qrCodeGenerator.GenerateQrCode();
        }
        /// <summary> Rysowanie markerów AruCo na arkuszu. </summary>
        private void DrawAruCoMarkers(XGraphics gfx, PdfPage page)
        {
            int markerId = 0;
            int x_margin = 10, y_margin = 10;
            KeyValuePair<double, double>[] pairs = {
                new KeyValuePair<double, double>(x_margin, y_margin),
                new KeyValuePair<double, double>(page.Width - 100 + 15, y_margin),
                new KeyValuePair<double, double>(page.Width - 100 + 15, page.Height - 100 + 15),
                new KeyValuePair<double, double>(x_margin, page.Height - 100 + 15)
            };
            foreach (var marker in _arrays_aruco)
            {
                var map = marker.GetMat().Bitmap;

                using (BufferedStream memory = new BufferedStream(new MemoryStream()))
                {
                    map.Save(memory, ImageFormat.Bmp);
                    var pair = pairs[markerId++];
                    gfx.DrawImage(XImage.FromStream(memory), pair.Key, pair.Value);
                    gfx.Save();
                }
            }
        }
        /// <summary> Rysowanie QRCodu na arkuszu. </summary>
        private void DrawQRCode(XGraphics gfx, PdfPage page)
        {
            using (BufferedStream memory = new BufferedStream(new MemoryStream()))
            {
                _qrcode.Save(memory, ImageFormat.Bmp);
                gfx.DrawImage(XImage.FromStream(memory), new XPoint(page.Width / 4 - 0.5 * _qrcode.Width, page.Height / 4 - _qrcode.Height));
                gfx.Save();
            }
        }
        #endregion

        /// <summary> Generowanie tablicy na odpowiedzi. </summary>
        private void GenerateAnswerTable(int question_number, int answer_number)
        {
            AnswerTableGenerator answerTableGenerator = new AnswerTableGenerator(question_number, answer_number, 5);
            _answer_table = answerTableGenerator.CreateStudentAnswerTable();

            AnswerTableRulesGenerator answerTableRulesGenerator = new AnswerTableRulesGenerator("13.5cm", "2cm");
            _answer_table_rules = answerTableRulesGenerator.GenerateAnswerTableRules();
        }
        /// <summary> Generowanie szablonu numeru albumu. </summary>
        private void GenerateIndexTable()
        {
            IndexGenerator studentIndexGenerator = new IndexGenerator();
            _index_table = studentIndexGenerator.CreateStudentIndexTable();

            IndexRulesGenerator indexRulesGenerator = new IndexRulesGenerator("8cm", "3cm");
            _index_table_rules = indexRulesGenerator.GenerateIndexRules();
        }
        /// <summary> Generowanie tabeli na podstawowe dane studenta. </summary>
        private void GenerateStudentInfoTable(double available_points)
        {
            StudentInfoTableGenerator studentInfo = new StudentInfoTableGenerator();
            _student_info_table = studentInfo.CreateStudentDataTable(available_points);
        }

        #region PAGES
        /// <summary> Tworzenie karty odpowiedzi. </summary>
        private void CreateAnswerSheetPage(String sheetID, String subject_name, String comment)
        {
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.MUH = PdfFontEncoding.Unicode;

            Document doc = new Document();
            Section section = doc.AddSection();

            //exam title
            TextFrame textFrame = new TextFrame();
            textFrame.Width = page.Width / 2;
            Paragraph p = textFrame.AddParagraph();
            p.AddFormattedText("Egzamin z przedmiotu: " + subject_name, TextFormat.Bold);
            p = textFrame.AddParagraph();
            p.AddFormattedText(comment, TextFormat.Bold);
            p = textFrame.AddParagraph();
            p.AddFormattedText("Identyfikator karty: " + sheetID, TextFormat.Bold);
            section.Add(textFrame);

            //student info table
            section.Add(_student_info_table);
            //index table
            section.Add(_index_table);
            section.Add(_index_table_rules);
            //answer table
            section.Add(_answer_table);
            section.Add(_answer_table_rules);

            DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            //markers
            DrawAruCoMarkers(gfx, page);
            //qrcode
            DrawQRCode(gfx, page);
            //title
            docRenderer.RenderObject(gfx, XUnit.FromPoint(100), XUnit.FromPoint(100 * 0.3), page.Width, textFrame);
            //student info table
            docRenderer.RenderObject(gfx, XUnit.FromPoint(page.Width / 3), XUnit.FromPoint(100), page.Width, _student_info_table);
            //render index
            docRenderer.RenderObject(gfx, XUnit.FromPoint(30), XUnit.FromPoint(200), page.Width, _index_table);
            docRenderer.RenderObject(gfx, XUnit.FromPoint(330), XUnit.FromPoint(210), page.Width, _index_table_rules);
            //studnet answer table
            docRenderer.RenderObject(gfx, XUnit.FromPoint(30), XUnit.FromPoint(300), page.Width, _answer_table);
            docRenderer.RenderObject(gfx, XUnit.FromPoint(100), XUnit.FromPoint(760), page.Width, _answer_table_rules);
        }
        /// <summary> Tworzenie brudnopisu. </summary>
        private void CreateRoughPage(String subject_name)
        {
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            gfx.MUH = PdfFontEncoding.Unicode;

            Document doc = new Document();
            Section section = doc.AddSection();

            Paragraph paragraph = section.Headers.Primary.AddParagraph();
            paragraph.AddFormattedText("Egzamin z przedmiotu: " + subject_name, TextFormat.Bold);
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            var answ_tabl_cl = _answer_table.Clone();
            var id_tab_cl = _index_table.Clone();
            section.Add(answ_tabl_cl);
            section.Add(id_tab_cl);

            DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            //title
            docRenderer.RenderObject(gfx, XUnit.FromPoint(0), XUnit.FromPoint(100 * 0.3), page.Width, paragraph);
            //render index
            docRenderer.RenderObject(gfx, XUnit.FromPoint(30), XUnit.FromPoint(200), page.Width, _index_table);
            //studnet answer table
            docRenderer.RenderObject(gfx, XUnit.FromPoint(30), XUnit.FromPoint(300), page.Width, _answer_table);
        }
        #endregion
    }
}
