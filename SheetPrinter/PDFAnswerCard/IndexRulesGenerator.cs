using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    class IndexRulesGenerator
    {
        public readonly string _width;
        public readonly string _height;

        private readonly String[] rules =
        {
        "1)Wpisanie numeru indeksu polega na zamalowaniu odpowiednich segmentów wyświetlacza.",
        "2)Student powinien zamalowywać segmenty w taki sposób, aby cyfra stanowiła jeden, spójny element" +
        " (by linie między segmentami były połączone w widoczny sposób).",
        "3)Student powinien zamalowywać poziome i pionowe segmenty szablonu po lewej stronie zgodnie ze wzorcem podanym poniżej:"
        };

        public IndexRulesGenerator(string width, string height)
        {
            _width = width;
            _height = height;
        }

        public TextFrame GenerateIndexRules()
        {
            TextFrame textframe = new TextFrame();
            textframe.Height = _height;
            textframe.Width = _width;

            foreach (String rule in rules)
            {
                Paragraph p = textframe.AddParagraph();
                p.AddFormattedText(rule);
                p.Format.Font.Size = 6;
                p.Format.Alignment = ParagraphAlignment.Justify;
                p.AddLineBreak();
            }

            Image image = textframe.AddImage("indexrule.png");
            image.ScaleWidth = 0.5;
            return textframe;
        }
    }
}
