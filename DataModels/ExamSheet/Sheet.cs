using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamChecker.SheetGenerator.DataModels.ExamSheet
{
    /// <summary>
    /// Klasa opisująca arkusz egzaminacyjny.
    /// Arkusz taki powinien zostać wygenerowany przez generator, na podstawie informacji z bazy danych.
    /// Po wygenerowaniu dane wygenerowanego arkusza muszą być przechowywane.
    /// </summary>
    public class Sheet
    {
        /// <summary> Unikalny identyfikator arkusza. </summary>
        public int SheetID { get; set; }

        /// <summary> Przedmiot, którego dotyczy ten arkusz egzaminacyjny. </summary>
        public Subject Subject { get; set; }

        /// <summary> Czas i data wygenerowania arkusza </summary>
        public DateTime GenerationTimestamp { get; set; }


        /// <summary> Komentarz do arkusza egzaminacyjnego; np. "Termin 2, Czwartek 16:15-18:00, aula E1" </summary>
        public string Comment { get; set; }


        /// <summary> Kolekcja pytań </summary>
        public QuestionBase[] Questions { get; set; }


        /// <summary>
        /// Maksymalna liczba punktów, które można otrzymać za odpowiedzi zgodne ze wzorcem w całym egzamienie.
        /// </summary>
        public double AvailablePoints => Questions.Select(x=>x.AvailablePoints).Sum();

        /// <summary> Konstruktor arkusza egzaminacyjnego; arkusz nie może istnieć bez identyfikatora </summary>
        /// <param name="sheetId">Identyfikator arkusza egzaminacyjnego</param>
        public Sheet(int sheetId)
        {
            this.SheetID = sheetId;
        }
    }
}