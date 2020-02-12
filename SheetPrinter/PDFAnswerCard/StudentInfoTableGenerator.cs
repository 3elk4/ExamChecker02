using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    class StudentInfoTableGenerator
    {
        private const double TABLE_THICKNESS = 0.5;

        public StudentInfoTableGenerator()
        {

        }

        //funcja do tworzenia tabeli na dane studenta
        public Table CreateStudentDataTable(double available_points)
        {
            Table table = new Table();
            table.Style = "Table";
            table.Format.Alignment = ParagraphAlignment.Center;
            table.Borders.Width = TABLE_THICKNESS;

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("2cm");
            column = table.AddColumn("2cm");
            column = table.AddColumn("2cm");
            column = table.AddColumn("2cm");
            column = table.AddColumn("2cm");
            column = table.AddColumn("2cm");

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Right;
            row.Format.Font.Bold = true;
            Paragraph p = row.Cells[0].AddParagraph();
            p.AddFormattedText("Imię i nazwisko studenta:");
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[1].MergeRight = 2;
            p = row.Cells[4].AddParagraph();
            p.AddFormattedText("Numer grupy:");
            row.Cells[4].VerticalAlignment = VerticalAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Right;
            row.Format.Font.Bold = true;
            p = row.Cells[0].AddParagraph();
            p.AddFormattedText("Ilość zdobytych punktów:");
            row.Cells[0].VerticalAlignment = VerticalAlignment.Center;
            p = row.Cells[2].AddParagraph();
            p.AddFormattedText("Punkty do zdobycia:");
            row.Cells[2].VerticalAlignment = VerticalAlignment.Center;
            row.Cells[3].AddParagraph(available_points.ToString());
            row.Cells[3].VerticalAlignment = VerticalAlignment.Center;
            p = row.Cells[4].AddParagraph();
            p.AddFormattedText("Ocena:");
            row.Cells[4].VerticalAlignment = VerticalAlignment.Center;
            return table;
        }
    }
}
