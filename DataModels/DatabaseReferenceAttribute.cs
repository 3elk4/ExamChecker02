using System;

namespace ExamChecker.SheetGenerator.DataModels
{
    /// <summary>
    /// Atrybut pomocniczy; określa pole będące referencją do bazy danych
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DatabaseReferenceAttribute : Attribute
    {
    }
}