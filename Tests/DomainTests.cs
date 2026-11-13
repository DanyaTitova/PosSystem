using PosSystem.Domain;
using Xunit;

namespace PosSystem.Tests;

public class DomainTests
{
    [Fact]
    public void Score_ExceedingMaxScore_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Score(12, 10));
    }

    [Fact]
    public void Score_NegativeValue_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Score(-1, 10));
    }

    [Fact]
    public void EvaluationFactory_ValidInput_CreatesEvaluation()
    {
        var factory = new EvaluationFactory();
        var student = new Student("Тестов Тест");
        var criterion = new Criterion("Семинары", 10, 1.5m);

        var eval = factory.Create(student, criterion, 9, "Отлично");

        Assert.Equal(9, eval.Score.Value);
        Assert.Equal(student.Id, eval.StudentId);
    }

    [Fact]
    public void GradeCalculationService_WeightedStrategy_ComputesCorrectFinalScore()
    {
        var factory = new EvaluationFactory();
        var student = new Student("Тестов Тест");
        var c1 = new Criterion("Критерий 1", 10, 1m); 
        var c2 = new Criterion("Критерий 2", 5, 3m);  

        var evals = new List<Evaluation>
        {
            factory.Create(student, c1, 5), 
            factory.Create(student, c2, 5) 
        };

        var service = new GradeCalculationService(new WeightedTenPointStrategy());
        decimal result = service.CalculateFinalScore(evals, new[] { c1, c2 });

        Assert.Equal(8.75m, result);
    }
}