using System.Net.Mime;

namespace ExamChecker.SheetGenerator.DataModels.ExamSheet
{
    /// <summary> Klasa opisująca jedną odpowiedź dla danego pytania </summary>
    public class Option
    {
        /// <summary> Identyfikator opcji do pytania </summary>
        [DatabaseReference]
        public int OptionID { get; set; }

        /// <summary> Opis opcji, w formacie Markdown </summary>
        public string Text { get; set; }

        /// <summary> Flaga określająca, czy student powinien oznaczyć tę odpowiedź. </summary>
        public bool MarkExpected { get; set; }


        /// <summary> Liczba punktów, jaką otrzyma student za poprawne zaznaczenie lub poprawne niezaznaczenie tej odpowiedzi. </summary>
        public double PointsForMatch { get; set; }

        /// <summary> Liczba punktów, jaką otrzyma student za niepoprawne zaznaczenie lub niepoprawne niezaznaczenie tej odpowiedzi. </summary>
        public double PointsForMismatch { get; set; }


        public Option(int id, string text, bool isMarkExpected, double matchPoints, double missmatchPoints)
        {
            this.OptionID = id;
            this.Text = text;
            this.MarkExpected = isMarkExpected;
            this.PointsForMatch = matchPoints;
            this.PointsForMismatch = missmatchPoints;
        }

        public Option(int id, string text, bool isMarkExpected)
            : this(id, text, isMarkExpected, 1, 0)
        {

        }

    }
}