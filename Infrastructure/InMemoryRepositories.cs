namespace PosSystem;

public class InMemoryStudentRepository : IStudentRepository
{
    private readonly List<Student> _items = new();
    public void Add(Student student) => _items.Add(student);
    public Student? GetById(Guid id) => _items.FirstOrDefault(s => s.Id == id);
    public IReadOnlyList<Student> GetAll() => _items.AsReadOnly();
    public void Remove(Guid id) => _items.RemoveAll(s => s.Id == id);
}

public class InMemoryCriterionRepository : ICriterionRepository
{
    private readonly List<Criterion> _items = new();
    public void Add(Criterion criterion) => _items.Add(criterion);
    public Criterion? GetById(Guid id) => _items.FirstOrDefault(c => c.Id == id);
    public IReadOnlyList<Criterion> GetAll() => _items.AsReadOnly();
}

public class InMemoryEvaluationRepository : IEvaluationRepository
{
    private readonly List<Evaluation> _items = new();
    public void Add(Evaluation evaluation) => _items.Add(evaluation);
    public Evaluation? Get(Guid studentId, Guid criterionId) =>
        _items.FirstOrDefault(e => e.StudentId == studentId && e.CriterionId == criterionId);
    public IReadOnlyList<Evaluation> GetByStudent(Guid studentId) =>
        _items.Where(e => e.StudentId == studentId).ToList().AsReadOnly();
    public void Remove(Guid evaluationId) => _items.RemoveAll(e => e.Id == evaluationId);
}