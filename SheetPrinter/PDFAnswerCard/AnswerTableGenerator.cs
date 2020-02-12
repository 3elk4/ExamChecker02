using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;

namespace ExamChecker.SheetGenerator.SheetPrinter.PDFAnswerCard
{
    class AnswerTableGenerator
    {
        public readonly String COLUMN_WIDTH = "0.6cm";
        public readonly String COLUMN_HEIGHT = "0.675cm";
        public readonly String ANSWER_BOX_WIDTH = "0.3cm";
        public readonly String ANSWER_BOX_HEIGHT = "0.3cm";
        public readonly String TABLE_SPACING = "0.2cm";
        public readonly double TABLE_ANSWER_THICKNESS = 1;
        public readonly double TABLE_THICKNESS = 0.5;

        public readonly int _question_number;
        public readonly int _answer_number;
        public readonly int _group_number; // ile ma dana grupa
        public readonly int _question_number_in_column; // max 20 w jednej kolumnie

        public AnswerTableGenerator(int question_number, int answer_number, int group_number)
        {
            if (group_number > question_number) group_number = 1;
            _question_number = question_number;
            _answer_number = answer_number;
            _group_number = (group_number == 4 || group_number == 5) ? group_number : 1;
            _question_number_in_column = 20;
        }

        public Table CreateStudentAnswerTable()
        {
            Table table = new Table();
            table.Style = "Table";
            table.Format.Alignment = ParagraphAlignment.Right;
            //table.Borders.Width = 0.5;

            Column column = null;
            Row row = null; Row inner_row = null;

            //int qnum = _answerTableInfo._question_number + 1;
            //int num = 0;
            int start = 0, temp_stop = _group_number, stop = 20, current_question_number = 0;
            int group_qunatity = stop / _group_number;
            //int group_stop = 0;

            int col_num = 5;
            string col_size = "3.7cm";
            if (_answer_number > 5 && _answer_number < 10)
            {
                col_num = 3;
                col_size = "6.16cm";
            }
            for(int i = 0; i < col_num; ++i)
            {
                column = table.AddColumn(col_size);
            } 
            row = table.AddRow();
            row.Height = "15cm";

            for (int c = 0; c < table.Columns.Count; ++c)
            {
                TextFrame textFrame = row.Cells[c].AddTextFrame();
                textFrame.Height = "15cm";
                start = 0;
                temp_stop = _group_number;

                for (int g = 0; g < group_qunatity; ++g) // ilość grup w danej kolumnie
                {
                    Table inner_table = textFrame.AddTable();
                    inner_table.Format.Alignment = ParagraphAlignment.Center;
                    inner_table.Style = "Table";

                    for (int i = 0; i <= _answer_number; ++i)
                    {
                        column = inner_table.AddColumn(COLUMN_WIDTH);
                        if (i != 0) column.Borders.Width = TABLE_ANSWER_THICKNESS;
                    }

                    for(int q = start; q <= temp_stop; ++q)
                    {
                        if (current_question_number == _question_number) break;
                        inner_row = inner_table.AddRow();
                        inner_row.Height = COLUMN_HEIGHT;
                        inner_row.Format.Alignment = ParagraphAlignment.Center;
                        inner_row.VerticalAlignment = VerticalAlignment.Center;
                        if (q == 0)
                        {
                            inner_row.Cells[0].AddParagraph("N");
                            for (int j = 1; j < inner_table.Columns.Count; ++j)
                            {
                                char answ = (char)('A' + (j - 1));
                                inner_row.Cells[j].AddParagraph(answ.ToString());
                            }
                        }
                        else
                        {
                            current_question_number++;
                            inner_row.Cells[0].AddParagraph(current_question_number.ToString());
                            for (int j = 1; j < inner_table.Columns.Count; ++j)
                            {
                                TextFrame temp_textFrame = inner_row.Cells[j].AddTextFrame();
                                temp_textFrame.Height = "0.3cm";
                                temp_textFrame.MarginLeft = "0.15cm";
                                Table answerBox = temp_textFrame.AddTable();
                                answerBox.Borders.Width = TABLE_THICKNESS;
                                answerBox.AddColumn(ANSWER_BOX_WIDTH);
                                Row temp_row = answerBox.AddRow();
                                temp_row.HeightRule = RowHeightRule.Exactly;
                                temp_row.Height = ANSWER_BOX_HEIGHT;
                            }
                        }
                    }
                    start = temp_stop + 1;
                    temp_stop += _group_number;
                    Paragraph p = textFrame.AddParagraph();
                    p.Format.LineSpacingRule = LineSpacingRule.Exactly;
                    p.Format.LineSpacing = TABLE_SPACING;
                }
            }

            return table;
        }
        /*
        public Table CreateStudentAnswerTable()
        {
            Table table = new Table();
            table.Style = "Table";
            table.Format.Alignment = ParagraphAlignment.Right;
            //table.Borders.Width = 0.5;

            Column column = null;
            Row row = null; Row inner_row = null;

            int qnum = _answerTableInfo._question_number + 1;
            int num = 0;
            int start = 0, stop = (int)((double)qnum / 2.0);
            int group_stop = 0;

            column = table.AddColumn("3.75cm");
            column = table.AddColumn("3.75cm");
            column = table.AddColumn("3.75cm");
            column = table.AddColumn("3.75cm");
            column = table.AddColumn("3.75cm");
            row = table.AddRow();
            row.Height = "15cm";

            for (int r = 0; r < 5; ++r)
            {
                start = 0;
                num = stop * r;
                group_stop = _answerTableInfo._question_number_in_group;
                TextFrame textFrame = row.Cells[r].AddTextFrame();
                textFrame.Height = "20cm";

                for (int g = 0; g < _answerTableInfo._group_number; ++g)
                {
                    Table inner_table = textFrame.AddTable();
                    inner_table.Format.Alignment = ParagraphAlignment.Center;
                    inner_table.Style = "Table";

                    for (int i = 0; i <= _answerTableInfo._answer_number; ++i)
                    {
                        column = inner_table.AddColumn(_answerTableInfo.COLUMN_WIDTH);
                        if (i != 0) column.Borders.Width = _answerTableInfo.TABLE_ANSWER_THICKNESS;
                    }

                    for (int i = start; i <= group_stop; ++i)
                    {
                        if ((i + num) > _answerTableInfo._question_number || i > stop) break;
                        inner_row = inner_table.AddRow();
                        inner_row.Height = _answerTableInfo.COLUMN_HEIGHT;
                        inner_row.Format.Alignment = ParagraphAlignment.Center;
                        inner_row.VerticalAlignment = VerticalAlignment.Center;
                        if (i == 0)
                        {
                            inner_row.Cells[0].AddParagraph("N");
                            for (int j = 1; j < inner_table.Columns.Count; ++j)
                            {
                                char answ = (char)('A' + (j - 1));
                                inner_row.Cells[j].AddParagraph(answ.ToString());
                            }
                        }
                        else
                        {
                            inner_row.Cells[0].AddParagraph((i + num).ToString());
                            for (int j = 1; j < inner_table.Columns.Count; ++j)
                            {
                                TextFrame temp_textFrame = inner_row.Cells[j].AddTextFrame();
                                temp_textFrame.Height = "0.5cm";
                                temp_textFrame.MarginLeft = "0.15cm";
                                Table answerBox = temp_textFrame.AddTable();
                                answerBox.Borders.Width = _answerTableInfo.TABLE_THICKNESS;
                                answerBox.AddColumn(_answerTableInfo.ANSWER_BOX_WIDTH);
                                Row temp_row = answerBox.AddRow();
                                temp_row.HeightRule = RowHeightRule.Exactly;
                                temp_row.Height = _answerTableInfo.ANSWER_BOX_HEIGHT;
                            }
                        }
                    }
                    start = group_stop + 1;
                    group_stop += _answerTableInfo._question_number_in_group;
                    Paragraph p = textFrame.AddParagraph();
                    p.Format.LineSpacingRule = LineSpacingRule.Exactly;
                    p.Format.LineSpacing = _answerTableInfo.TABLE_SPACING;
                }
                
                //stop = qnum;
            }

            return table;
        }*/

        /* public TextFrame CreateStudentAnswerTable()
         {
             TextFrame textFrame = new TextFrame();
             int start = 0, stop = _answerTableInfo._question_number_in_group;

             for(int g = 0; g < _answerTableInfo._group_number; ++g)
             {
                 Table table = new Table();
                 table.Format.Alignment = ParagraphAlignment.Center;
                 table.Style = "Table";

                 Column column = null;
                 Row row = null;
                 //make column number
                 for (int i = 0; i <= _answerTableInfo._answer_number; ++i)
                 {
                     column = table.AddColumn(_answerTableInfo.COLUMN_WIDTH);
                     if (i != 0) column.Borders.Width = _answerTableInfo.TABLE_ANSWER_THICKNESS;
                 }

                 //answers
                 for (int i = start; i <= stop; ++i)
                 {
                     if (i > _answerTableInfo._question_number) break;
                     row = table.AddRow();
                     row.Height = _answerTableInfo.COLUMN_HEIGHT;
                     row.Format.Alignment = ParagraphAlignment.Center;
                     row.VerticalAlignment = VerticalAlignment.Center;
                     if (i == 0)
                     {
                         row.Cells[0].AddParagraph("N");
                         for (int j = 1; j < table.Columns.Count; ++j)
                         {
                             char answ = (char)('A' + (j - 1));
                             row.Cells[j].AddParagraph(answ.ToString());
                         }
                     }
                     else
                     {
                         row.Cells[0].AddParagraph(i.ToString());
                         for (int j = 1; j < table.Columns.Count; ++j)
                         {
                             TextFrame temp_textFrame = row.Cells[j].AddTextFrame();
                             temp_textFrame.Height = "0.5cm";
                             temp_textFrame.MarginLeft = "0.15cm";
                             Table answerBox = temp_textFrame.AddTable();
                             answerBox.Borders.Width = _answerTableInfo.TABLE_THICKNESS;
                             answerBox.AddColumn(_answerTableInfo.ANSWER_BOX_WIDTH);
                             Row temp_row = answerBox.AddRow();
                             temp_row.HeightRule = RowHeightRule.Exactly;
                             temp_row.Height = _answerTableInfo.ANSWER_BOX_HEIGHT;
                         }
                     }
                 }
                 start = stop + 1;
                 stop += _answerTableInfo._question_number_in_group;
                 textFrame.Add(table);
                 Paragraph p = textFrame.AddParagraph();
                 p.Format.LineSpacingRule = LineSpacingRule.Exactly;
                 p.Format.LineSpacing = _answerTableInfo.TABLE_SPACING;
             }

             return textFrame;
         }*/
    }
}
