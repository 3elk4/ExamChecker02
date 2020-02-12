namespace ExamChecker.SheetGenerator.DataModels
{
    public class Student
    {
        /// <summary> Identyfikator w bazie danych </summary>
        [DatabaseReference]
        public int StudentID { get; set; }

        /// <summary> Imię i nazwisko studenta </summary>
        public string FullName { get; set; }

        /// <summary> Numer albumu </summary>
        public string Number { get; set; }
    }
}