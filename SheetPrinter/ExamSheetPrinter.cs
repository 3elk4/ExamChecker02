using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamChecker.SheetGenerator.DataModels.ExamSheet;

namespace ExamChecker.SheetGenerator.SheetPrinter
{
    /// <summary> Klasa bazowa wydruków. </summary>
    public abstract class ExamSheetPrinter
    {
        protected ExamSheetPrinter()
        {

        }

        /// <summary> Wydruk jednego arkusza. </summary>
        public abstract void Print(Sheet sheet);
        /// <summary> Wydruk wielu arkuszy. </summary>
        public abstract void PrintAll(List<Sheet> sheets);
        
    }
}
