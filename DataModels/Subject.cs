namespace ExamChecker.SheetGenerator.DataModels
{
    /// <summary> Klasa opisująca przedmiot. </summary>
    public class Subject
    {
        /// <summary> Unikalny identyfikator przedmiotu w bazie danych </summary>
        public int SubjectID { get; set; }

        /// <summary> Nazwa przedmiotu </summary>
        public string Name { get; set; }


        public Subject(int subjectId, string name)
        {
            this.SubjectID = subjectId;
            this.Name = name;
        }
    }
}