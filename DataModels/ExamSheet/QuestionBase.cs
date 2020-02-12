namespace ExamChecker.SheetGenerator.DataModels.ExamSheet
{
    /// <summary> Klasa bazowa pytania </summary>
    public abstract class QuestionBase
    {
        /// <summary> Identyfikator pytania w bazie </summary>
        [DatabaseReference]
        public int QuestionID { get; set; }

        /// <summary> Treść pytania w formacie Markdown. </summary>
        public string Text { get; set; }

        public abstract double AvailablePoints { get; }


        protected QuestionBase(int questionId)
        {
            this.QuestionID = questionId;
        }
    }
}