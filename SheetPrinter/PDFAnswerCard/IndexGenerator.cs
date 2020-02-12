using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    class IndexGenerator
    {
        public readonly String COLUMN_WIDTH = "1.7cm";
        public readonly String COLUMN_HEIGHT = "3cm";
        public readonly String CELL_WIDTH = "0.9cm";
        public readonly String CELL_HEIGHT = "1cm";
        public readonly double CELL_THICKNESS = 1.5;

        public IndexGenerator()
        {
        }

        public TextFrame CreateStudentIndexTable()
        {
            TextFrame textFrame = new TextFrame();
            textFrame.AddParagraph("Numer indeksu:");
            Table table = textFrame.AddTable();
            table.Format.Alignment = ParagraphAlignment.Center;
            table.Style = "Table";

            Column column = null;
            Row row = null;
            for (int i = 0; i < 6; ++i)
            {
                column = table.AddColumn(COLUMN_WIDTH);
            }

            row = table.AddRow();
            row.Height = COLUMN_HEIGHT;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.VerticalAlignment = VerticalAlignment.Center;
           
            for (int i = 0; i < 6; ++i)
            {
                TextFrame tempframe = row.Cells[i].AddTextFrame();
                tempframe.Height = COLUMN_HEIGHT;
                tempframe.Width = COLUMN_WIDTH;
                tempframe.MarginTop = "0.5cm";
                tempframe.MarginLeft = "0.4cm";

                Table indexCell = tempframe.AddTable();
                indexCell.Borders.Style = BorderStyle.DashLargeGap;
                indexCell.Borders.Color = Color.FromCmyk(50, 50, 50, 30);
                indexCell.Borders.Width = CELL_THICKNESS;
                indexCell.AddColumn(CELL_WIDTH);
                for(int r = 0; r < 2; ++r)
                {
                    Row temp_row = indexCell.AddRow();
                    temp_row.HeightRule = RowHeightRule.Exactly;
                    temp_row.Height = CELL_HEIGHT;
                }
            }

            return textFrame;
        }
    }
}
