namespace PosSystem;

public interface IGradeStrategy
{
    decimal Calculate(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> criteria);
}

public class WeightedTenPointStrategy : IGradeStrategy
{
    public decimal Calculate(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> criteria)
    {
        var criteriaMap = criteria.ToDictionary(c => c.Id);
        decimal weightedSum = 0;
        decimal totalWeight = 0;

        foreach (var eval in evaluations)
        {
            if (!criteriaMap.TryGetValue(eval.CriterionId, out var criterion))
                continue;

            decimal normalized = (eval.Score.Value / criterion.MaxScore) * 10m;
            weightedSum += normalized * criterion.Weight.Value;
            totalWeight += criterion.Weight.Value;
        }

        return totalWeight == 0 ? 0 : Math.Round(weightedSum / totalWeight, 2);
    }
}

public class RawSumStrategy : IGradeStrategy
{
    public decimal Calculate(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> criteria)
    {
        var validIds = criteria.Select(c => c.Id).ToHashSet();
        return evaluations.Where(e => validIds.Contains(e.CriterionId)).Sum(e => e.Score.Value);
    }
}

public class GradeCalculationService
{
    private IGradeStrategy _strategy;

    public GradeCalculationService(IGradeStrategy defaultStrategy)
    {
        _strategy = defaultStrategy;
    }

    public void SetStrategy(IGradeStrategy strategy) => _strategy = strategy;

    public decimal CalculateFinalScore(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> criteria)
    {
        return _strategy.Calculate(evaluations, criteria);
    }
}