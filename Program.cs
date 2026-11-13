using Microsoft.Extensions.DependencyInjection;
using PosSystem.Application;
using PosSystem.Domain;
using PosSystem.Infrastructure;

namespace PosSystem;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var services = new ServiceCollection();
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<ICriterionRepository, InMemoryCriterionRepository>();
        services.AddSingleton<IEvaluationRepository, InMemoryEvaluationRepository>();
        services.AddSingleton<IReportExporter, CsvReportExporter>();
        services.AddSingleton<EvaluationFactory>();
        services.AddSingleton<IGradeStrategy, WeightedTenPointStrategy>();
        services.AddSingleton<GradeCalculationService>();
        services.AddSingleton<PosFacade>();

        var provider = services.BuildServiceProvider();
        var pos = provider.GetRequiredService<PosFacade>();

        // Автоматический сквозной сценарий 
        RunDemoScenario(pos);

        // Интерактивное меню
        RunInteractiveMenu(pos);
    }

    static void RunDemoScenario(PosFacade pos)
    {
        Console.WriteLine("=== СКВОЗНОЙ ДЕМО-СЦЕНАРИЙ ===");
        var s1 = pos.AddStudent("Иванов Иван");
        var s2 = pos.AddStudent("Петрова Анна");

        var c1 = pos.AddCriterion("Посещал лекции", 5, 1.0m);
        var c2 = pos.AddCriterion("Не нарушал дедлайны", 10, 2.0m);
        var c3 = pos.AddCriterion("ИИ использовал, но понял сам", 10, 3.0m);

        pos.SetOrUpdateEvaluation(s1.Id, c1.Id, 5, "Все лекции");
        pos.SetOrUpdateEvaluation(s1.Id, c2.Id, 8, "Один перенос");
        pos.SetOrUpdateEvaluation(s1.Id, c3.Id, 10, "Защитил уверенно");

        pos.SetOrUpdateEvaluation(s2.Id, c1.Id, 4);
        pos.SetOrUpdateEvaluation(s2.Id, c2.Id, 10);
        pos.SetOrUpdateEvaluation(s2.Id, c3.Id, 9);

        Console.WriteLine($"Итог {s1.FullName}: {pos.GetStudentFinalScore(s1.Id)} / 10");
        Console.WriteLine($"Итог {s2.FullName}: {pos.GetStudentFinalScore(s2.Id)} / 10");

        // Проверка инвариантов
        try
        {
            pos.SetOrUpdateEvaluation(s1.Id, c1.Id, 12); 
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"[Защита инварианта 1]: {ex.Message}");
        }

        try
        {
            pos.SetOrUpdateEvaluation(s1.Id, Guid.NewGuid(), 5); 
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[Защита инварианта 2]: {ex.Message}");
        }

        string path = pos.ExportSummaryReport();
        Console.WriteLine($"Ведомость выгружена: {path}\n");
    }

    static void RunInteractiveMenu(PosFacade pos)
    {
        while (true)
        {
            Console.WriteLine("--- МЕНЮ ПОС ---");
            Console.WriteLine("1. Добавить студента");
            Console.WriteLine("2. Добавить критерий");
            Console.WriteLine("3. Выставить / изменить оценку");
            Console.WriteLine("4. Удалить оценку");
            Console.WriteLine("5. Показать ведомость и итоги");
            Console.WriteLine("6. Экспортировать итог в CSV");
            Console.WriteLine("0. Выход");
            Console.Write("Выбор: ");

            var choice = Console.ReadLine();
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("ФИО студента: ");
                        pos.AddStudent(Console.ReadLine() ?? "");
                        Console.WriteLine("Студент добавлен.\n");
                        break;

                    case "2":
                        Console.Write("Название критерия: ");
                        string name = Console.ReadLine() ?? "";
                        Console.Write("Макс. балл: ");
                        decimal max = decimal.Parse(Console.ReadLine() ?? "0");
                        Console.Write("Вес критерия: ");
                        decimal weight = decimal.Parse(Console.ReadLine() ?? "1");
                        pos.AddCriterion(name, max, weight);
                        Console.WriteLine("Критерий добавлен.\n");
                        break;

                    case "3":
                        var students = pos.GetAllStudents();
                        var criteria = pos.GetAllCriteria();
                        for (int i = 0; i < students.Count; i++)
                            Console.WriteLine($"{i + 1}. {students[i].FullName}");
                        Console.Write("Номер студента: ");
                        var st = students[int.Parse(Console.ReadLine() ?? "1") - 1];

                        for (int i = 0; i < criteria.Count; i++)
                            Console.WriteLine($"{i + 1}. {criteria[i].Name} (макс {criteria[i].MaxScore})");
                        Console.Write("Номер критерия: ");
                        var cr = criteria[int.Parse(Console.ReadLine() ?? "1") - 1];

                        Console.Write("Балл: ");
                        decimal score = decimal.Parse(Console.ReadLine() ?? "0");
                        Console.Write("Комментарий (Enter чтобы пропустить): ");
                        string? comment = Console.ReadLine();

                        pos.SetOrUpdateEvaluation(st.Id, cr.Id, score, string.IsNullOrWhiteSpace(comment) ? null : comment);
                        Console.WriteLine("Оценка сохранена.\n");
                        break;

                    case "4":
                        var stList = pos.GetAllStudents();
                        var crList = pos.GetAllCriteria();
                        for (int i = 0; i < stList.Count; i++) Console.WriteLine($"{i + 1}. {stList[i].FullName}");
                        Console.Write("Номер студента: ");
                        var sDel = stList[int.Parse(Console.ReadLine() ?? "1") - 1];

                        for (int i = 0; i < crList.Count; i++) Console.WriteLine($"{i + 1}. {crList[i].Name}");
                        Console.Write("Номер критерия: ");
                        var cDel = crList[int.Parse(Console.ReadLine() ?? "1") - 1];

                        pos.RemoveEvaluation(sDel.Id, cDel.Id);
                        Console.WriteLine("Оценка отменена.\n");
                        break;

                    case "5":
                        foreach (var s in pos.GetAllStudents())
                        {
                            Console.WriteLine($"Студент: {s.FullName} | Итог: {pos.GetStudentFinalScore(s.Id)}");
                            foreach (var e in pos.GetStudentEvaluations(s.Id))
                            {
                                var cName = pos.GetAllCriteria().First(c => c.Id == e.CriterionId).Name;
                                Console.WriteLine($"   - {cName}: {e.Score.Value} ({e.Comment ?? "без комментария"})");
                            }
                        }
                        Console.WriteLine();
                        break;

                    case "6":
                        Console.WriteLine($"Сохранено в файл: {pos.ExportSummaryReport()}\n");
                        break;

                    case "0":
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}\n");
            }
        }
    }
}