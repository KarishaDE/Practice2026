using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public static class StudentJsonService
{
    private static readonly JsonSerializerOptions Options = CreateOptions();

    public static string Serialize(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        return JsonSerializer.Serialize(student, Options);
    }

    public static Student Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON не должен быть пустым", nameof(json));
        }

        var student = JsonSerializer.Deserialize<Student>(json, Options)
            ?? throw new InvalidDataException("Не удалось получить данные студента");

        Validate(student);
        return student;
    }

    public static void SaveToFile(Student student, string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не должен быть пустым", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(filePath, Serialize(student));
    }

    public static Student LoadFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Путь к файлу не должен быть пустым", nameof(filePath));
        }

        return Deserialize(File.ReadAllText(filePath));
    }

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        options.Converters.Add(new StudentDateTimeConverter());
        return options;
    }

    private static void Validate(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.FirstName))
        {
            throw new InvalidDataException("Имя студента не указано");
        }

        if (string.IsNullOrWhiteSpace(student.LastName))
        {
            throw new InvalidDataException("Фамилия студента не указана");
        }

        if (student.BirthDate == default || student.BirthDate.Date > DateTime.Today)
        {
            throw new InvalidDataException("Дата рождения указана неверно");
        }

        if (student.Grades is null)
        {
            throw new InvalidDataException("Список оценок не указан");
        }

        if (student.Grades.Any(subject =>
                subject is null || string.IsNullOrWhiteSpace(subject.Name)))
        {
            throw new InvalidDataException("Название предмета не указано");
        }

        if (student.Grades.Any(subject => subject.Grade is < 1 or > 5))
        {
            throw new InvalidDataException("Оценка должна быть от 1 до 5");
        }
    }
}
