using task13;

var filePath = args.Length > 0
    ? args[0]
    : Path.Combine(Environment.CurrentDirectory, "student.json");

var student = new Student
{
    FirstName = "Анна",
    LastName = "Иванова",
    BirthDate = new DateTime(2005, 4, 15),
    Grades = new List<Subject>
    {
        new() { Name = "Математика", Grade = 5 },
        new() { Name = "Физика", Grade = 4 }
    }
};

StudentJsonService.SaveToFile(student, filePath);
var loadedStudent = StudentJsonService.LoadFromFile(filePath);

Console.WriteLine($"JSON сохранён в файл: {filePath}");
Console.WriteLine(StudentJsonService.Serialize(loadedStudent));
