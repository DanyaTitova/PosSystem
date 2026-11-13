using System.Text;

namespace PosSystem;

public class CsvReportExporter : IReportExporter
{
    public string Export(
        IEnumerable<(Student Student, decimal FinalScore, IReadOnlyList<Evaluation> Evals)> data,
        IReadOnlyList<Criterion> criteria)
    {
        var sb = new StringBuilder();
        var header = "Студент;" + string.Join(";", criteria.Select(c => $"{c.Name} (макс {c.MaxScore})")) + ";Итог";
        sb.AppendLine(header);

        foreach (var row in data)
        {
            var evalMap = row.Evals.ToDictionary(e => e.CriterionId, e => e.Score.Value.ToString());
            var scores = criteria.Select(c => evalMap.GetValueOrDefault(c.Id, "-"));
            sb.AppendLine($"{row.Student.FullName};{string.Join(";", scores)};{row.FinalScore}");
        }

        string fileName = "pos_report.csv";
        File.WriteAllText(fileName, sb.ToString(), Encoding.UTF8);
        return Path.GetFullPath(fileName);
    }
}