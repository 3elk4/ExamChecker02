using System.Collections.Generic;
using System.Linq;

namespace ExamChecker.SheetGenerator.DataModels.ExamSheet
{
    /// <summary> Klasa opisująca pytanie jednokrotnego wyboru. </summary>
    public class SingleChoiceQuestion : QuestionBase
    {
        /// <summary> Lista uporządkowana pytań; tylko jedno powinno być zaznaczalne. </summary>
        public Option[] Options { get; set; }

        /// <summary> Odpowiedź z kolekcji <see cref="Options"/>, która powinna zostać zaznaczona. </summary>
        public Option MarkableOption => Options.FirstOrDefault(x => x.MarkExpected);

        /// <summary> Liczba punktów do zdobycia za zaznaczenie zgodne ze wzorcem. </summary>
        public override double AvailablePoints => Options.Select(x => x.PointsForMatch).Sum();


        public SingleChoiceQuestion(int questionId)
            : base(questionId)
        {
            //
        }
    }
}