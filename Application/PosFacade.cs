namespace PosSystem;

public class PosFacade
{
    private readonly IStudentRepository _students;
    private readonly ICriterionRepository _criteria;
    private readonly IEvaluationRepository _evaluations;
    private readonly EvaluationFactory _factory;
    private readonly GradeCalculationService _gradeService;
    private readonly IReportExporter _exporter;

    public PosFacade(
        IStudentRepository students,
        ICriterionRepository criteria,
        IEvaluationRepository evaluations,
        EvaluationFactory factory,
        GradeCalculationService gradeService,
        IReportExporter exporter)
    {
        _students = students;
        _criteria = criteria;
        _evaluations = evaluations;
        _factory = factory;
        _gradeService = gradeService;
        _exporter = exporter;
    }

    public Student AddStudent(string fullName)
    {
        var student = new Student(fullName);
        _students.Add(student);
        return student;
    }

    public Criterion AddCriterion(string name, decimal maxScore, decimal weight)
    {
        var criterion = new Criterion(name, maxScore, weight);
        _criteria.Add(criterion);
        return criterion;
    }

    public void SetOrUpdateEvaluation(Guid studentId, Guid criterionId, decimal score, string? comment = null)
    {
        var student = _students.GetById(studentId) ?? throw new InvalidOperationException("Студент не найден.");
        var criterion = _criteria.GetById(criterionId) ?? throw new InvalidOperationException("Критерий отсутствует в рубрикаторе.");

        var existing = _evaluations.Get(studentId, criterionId);
        if (existing != null)
        {
            existing.UpdateScore(score, criterion.MaxScore, comment);
            return;
        }

        var evaluation = _factory.Create(student, criterion, score, comment);
        _evaluations.Add(evaluation);
    }

    public void RemoveEvaluation(Guid studentId, Guid criterionId)
    {
        var existing = _evaluations.Get(studentId, criterionId) ?? throw new InvalidOperationException("Оценка не найдена.");
        _evaluations.Remove(existing.Id);
    }

    public decimal GetStudentFinalScore(Guid studentId)
    {
        var evals = _evaluations.GetByStudent(studentId);
        var criteria = _criteria.GetAll();
        return _gradeService.CalculateFinalScore(evals, criteria);
    }

    public void SwitchCalculationStrategy(IGradeStrategy strategy) => _gradeService.SetStrategy(strategy);

    public IReadOnlyList<Student> GetAllStudents() => _students.GetAll();
    public IReadOnlyList<Criterion> GetAllCriteria() => _criteria.GetAll();
    public IReadOnlyList<Evaluation> GetStudentEvaluations(Guid studentId) => _evaluations.GetByStudent(studentId);

    public string ExportSummaryReport()
    {
        var criteria = _criteria.GetAll();
        var reportData = _students.GetAll().Select(s => (
            Student: s,
            FinalScore: GetStudentFinalScore(s.Id),
            Evals: _evaluations.GetByStudent(s.Id)
        ));

        return _exporter.Export(reportData, criteria);
    }
}