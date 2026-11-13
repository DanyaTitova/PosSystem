namespace PosSystem.Domain;

public interface IStudentRepository
{
    void Add(Student student);
    Student? GetById(Guid id);
    IReadOnlyList<Student> GetAll();
    void Remove(Guid id);
}

public interface ICriterionRepository
{
    void Add(Criterion criterion);
    Criterion? GetById(Guid id);
    IReadOnlyList<Criterion> GetAll();
}

public interface IEvaluationRepository
{
    void Add(Evaluation evaluation);
    Evaluation? Get(Guid studentId, Guid criterionId);
    IReadOnlyList<Evaluation> GetByStudent(Guid studentId);
    void Remove(Guid evaluationId);
}

public interface IReportExporter
{
    string Export(IEnumerable<(Student Student, decimal FinalScore, IReadOnlyList<Evaluation> Evals)> data, IReadOnlyList<Criterion> criteria);
}