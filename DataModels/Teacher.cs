namespace ExamChecker.SheetGenerator.DataModels
{
    /// <summary> Klasa opisująca nauczyciela/wykładowcę </summary>
    public class Teacher
    {
        /// <summary> Identyfikator w bazie danych </summary>
        [DatabaseReference]
        public int TeacherID { get; set; }

        /// <summary> Imię i nazwisko </summary>
        public string FullName { get; set; }

        /// <summary> Adres email tej osoby </summary>
        public string Email { get; set; }
    }
}