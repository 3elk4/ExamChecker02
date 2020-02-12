using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamChecker.SheetGenerator.DataModels.ExamSheet;
using Markdig;
using Markdig.Extensions.ListExtras;

namespace ExamChecker.SheetGenerator.SheetPrinter
{
    /// <summary> Klasa odpowiedzialan za wydruk pytań i odpowiedzi w formie HTML. </summary>
    class HTMLExamSheetPrinter : ExamSheetPrinter
    {
        /// <summary> Nazwa pliku z rozszerzeniem HTML. </summary>
        string HTMLname;

        ///<summary> Dodaje rozszerzenie do markdowna i dzięki temu można poprawić stylistykę w HTML. </summary>
        private static MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        public HTMLExamSheetPrinter(string HTMLname)
        {
            this.HTMLname = HTMLname;
        }

        /// <summary> Wydruk jednego zestawu pytań do pliku. </summary>
        public override void Print(Sheet sheet)
        {
            using (StreamWriter sw = new StreamWriter(HTMLname))
            {
                var header_text = ExamSheetHeader(sheet);
                sw.WriteLine(header_text);

                var html_text = ExamSheetQuestions(sheet);
                sw.WriteLine(html_text);
            }
        }

        /// <summary> Wydruk wielu zestawów pytań do pliku. </summary>
        public override void PrintAll(List<Sheet> sheets)
        {
            using (StreamWriter sw = new StreamWriter(HTMLname))
            {
                foreach(var sheet in sheets)
                {
                    var header_text = ExamSheetHeader(sheet);
                    sw.WriteLine(header_text);

                    var html_text = ExamSheetQuestions(sheet);
                    sw.WriteLine(html_text.ToString());
                }
            }
        }

        /// <summary>
        /// Przekonwertowany zestaw pytań
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static string RawHTMLText(Sheet sheet)
        {
            StringBuilder text = new StringBuilder();
            StringBuilder temp_text = new StringBuilder();

            temp_text.Append("**Przedmiot: " + sheet.Subject.Name + "  \n");
            temp_text.Append(sheet.Comment +"  \n");
            temp_text.Append("Identyfikator arkusza: " + sheet.SheetID + "**");

            text.Append(Markdown.ToHtml(temp_text.ToString()));
            temp_text.Clear();
            int qid = 1;
            foreach (var question in sheet.Questions)
            {
                temp_text.Append(qid + ". " + question.Text).AppendLine();
                if (question is SingleChoiceQuestion _sq)
                    foreach (var answer in _sq.Options) temp_text.Append("\ta. " + answer.Text).AppendLine();
                else if (question is MultipleChoiceQuestion _mq)
                    foreach (var answer in _mq.Options) temp_text.Append("\ta. " + answer.Text).AppendLine();
                qid++;
            }
            text.Append(Markdown.ToHtml(temp_text.ToString(), pipeline));
            return text.ToString();
        }

        /// <summary> Nagłówek arkusza. </summary>
        private string ExamSheetHeader(Sheet sheet)
        {
            StringBuilder text = new StringBuilder();
            text.Append("**Identyfikator arkusza: " + sheet.SheetID + "**  \n");
            text.Append("**Przedmiot: " + sheet.Subject.Name + "**  \n");
            text.Append("**" + sheet.Comment + "**").AppendLine();

            return Markdown.ToHtml(text.ToString(), pipeline);
        }

        /// <summary> Konwersja zestawu pytań i odpowiedzi z Markdown do HTML. </summary>
        private StringBuilder ExamSheetQuestions(Sheet sheet)
        {
            StringBuilder text = new StringBuilder();
            StringBuilder temp_text = new StringBuilder();
            
            int qid = 1;
            foreach (var question in sheet.Questions)
            {
                temp_text.Append(qid + ". " + question.Text).AppendLine();
                if (question is SingleChoiceQuestion _sq)
                    foreach (var answer in _sq.Options) temp_text.Append("\ta. " + answer.Text).AppendLine();
                else if (question is MultipleChoiceQuestion _mq)
                    foreach (var answer in _mq.Options) temp_text.Append("\ta. " + answer.Text).AppendLine();
                qid++;
            }
            text.Append(Markdown.ToHtml(temp_text.ToString(), pipeline));
            return text;
        }
    }
}
