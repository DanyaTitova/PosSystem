namespace PosSystem.Domain;

// Единая точка создания оценки с гарантией соблюдения инвариантов рубрикатора
public class EvaluationFactory
{
    public Evaluation Create(Student student, Criterion criterion, decimal rawScore, string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(criterion);

        // Валидация диапазона  внутри Value Object
        var validScore = new Score(rawScore, criterion.MaxScore);
        return new Evaluation(student.Id, criterion.Id, validScore, comment);
    }
}