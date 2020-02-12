using System.Collections.Generic;
using System.Linq;

namespace ExamChecker.SheetGenerator.DataModels.ExamSheet
{
    /// <summary> Klasa opisująca pytanie wielokrotnego wyboru. </summary>
    public class MultipleChoiceQuestion : QuestionBase
    {
        /// <summary> Lista uporządkowana pytań; tylko jedno powinno być zaznaczalne. </summary>
        public Option[] Options { get; set; }

        /// <summary> Lista pytań które student powinien zaznaczyć. </summary>
        public Option[] MarkableOptions => Options.Where(x => x.MarkExpected).ToArray();

        /// <summary> Lista odpowiedzi, których student nie powinien zaznaczać </summary>
        public Option[] NonMarkableOptions => Options.Where(x => !x.MarkExpected).ToArray();

        /// <summary> Liczba punktów do zdobycia za zaznaczenie zgodne ze wzorcem. </summary>
        public override double AvailablePoints => Options.Select(x => x.PointsForMatch).Sum();



        public MultipleChoiceQuestion(int questionId)
            : base(questionId)
        {
            //
        }
    }
}