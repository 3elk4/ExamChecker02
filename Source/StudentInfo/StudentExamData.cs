using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamChecker.Source.StudentInfo
{
    ///<summary>Klasa przedstawiająca efekt przeparsowania zdjęcia przez assembly ImageProcessor - 
    ///Core aplikacji będzie mógł potem ocenić uzyskane dane z tej klasy poprzez komunikację z komponentem bazy danych</summary>
    class StudentExamData
    {
        /// <summary> Unikalny identyfikator arkusza z kodu QR. No właśnie - int czy string?
        /// GUID daje nam najwięcej losowości i szansy unikatów, no chyba, że weźmiemy silnik do
        /// liczb losowych z assembly Cryptography chyba z asp.net i będziemy te liczbowe Id generować </summary>
        public String SheetID { get; set; }

        /// <summary> Id studenta uzyskane dzięki OCR </summary>
        public String StudentID { get; set; }

        /// <summary> Kolekcja pytań z odpowiedziami - tutaj wziąłem typ, jaki doktor zostawił w klasie Document,
        /// aczkolwiek osobiście chyba byłbym zwolennikiem Dictionary<Question, List<Answer>> -
        /// łatwiej jest potem podobną kolekcję uzyskać z baz danych</summary>
        public List<Question> Answers { get; set; }

        public StudentExamData(String sheet_id, String index, List<Question> answers)
        {
            this.StudentID = index;
            this.SheetID = sheet_id;
            this.Answers = answers;
        }
        
    }
}
