namespace PosSystem.Domain;

public class Student
{
    public Guid Id { get; } = Guid.NewGuid();
    public string FullName { get; private set; }

    public Student(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("ФИО студента не может быть пустым.");
        FullName = fullName.Trim();
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Новое имя не может быть пустым.");
        FullName = newName.Trim();
    }
}

public class Criterion
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; private set; }
    public decimal MaxScore { get; private set; }
    public Weight Weight { get; private set; }

    public Criterion(string name, decimal maxScore, decimal weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название критерия не может быть пустым.");
        if (maxScore <= 0)
            throw new ArgumentException("Максимальный балл должен быть больше 0.");

        Name = name.Trim();
        MaxScore = maxScore;
        Weight = new Weight(weight);
    }
}

public class Evaluation
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid StudentId { get; }
    public Guid CriterionId { get; }
    public Score Score { get; private set; }
    public DateTime GradedAt { get; private set; }
    public string? Comment { get; private set; }

    internal Evaluation(Guid studentId, Guid criterionId, Score score, string? comment)
    {
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score;
        GradedAt = DateTime.Now;
        Comment = comment;
    }

    public void UpdateScore(decimal newRawScore, decimal maxScore, string? newComment = null)
    {
        Score = new Score(newRawScore, maxScore);
        GradedAt = DateTime.Now;
        if (newComment != null) Comment = newComment;
    }
}