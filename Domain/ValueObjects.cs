namespace PosSystem.Domain;

public sealed record Score
{
    public decimal Value { get; }

    public Score(decimal value, decimal maxScore)
    {
        if (value < 0 || value > maxScore)
            throw new ArgumentException($"Балл {value} выходит за пределы критерия [0..{maxScore}].");
        Value = value;
    }
}

public sealed record Weight
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        if (value <= 0)
            throw new ArgumentException("Вес критерия должен быть строго больше нуля.");
        Value = value;
    }
}