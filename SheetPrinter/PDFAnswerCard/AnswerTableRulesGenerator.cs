using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    class AnswerTableRulesGenerator
    {
        public readonly string _width;
        public readonly string _height;

        private readonly String[] rules =
        {
        "Zasady wypełniania odpowiedzi:",
        "Aby zaznaczyć wybraną odpowiedź należy zamalować mały kwadrat w odpowiedniej kolumnie i wierszu.",
        "Tak udzielona odpowiedź jest ostateczna - nie ma możliwości jej poprawy/zmiany."
        };

        public AnswerTableRulesGenerator(string width, string height)
        {
            this._height = height;
            this._width = width;
        }

        public TextFrame GenerateAnswerTableRules()
        {
            TextFrame textframe = new TextFrame();
            textframe.Height = _height;
            textframe.Width = _width;

            foreach (String rule in rules)
            {
                Paragraph p = textframe.AddParagraph();
                p.AddFormattedText(rule);
                p.Format.Font.Size = 10;
                p.Format.Alignment = ParagraphAlignment.Justify;
                p.AddLineBreak();
            }

            return textframe;
        }

    }
}
