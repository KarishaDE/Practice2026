using System.Text.Json;
using task13;

namespace task13tests;

public class StudentJsonServiceTests
{
    [Fact]
    public void Serialize_ShouldCreateJsonWithFormattedDate()
    {
        var student = CreateStudent();

        var json = StudentJsonService.Serialize(student);

        Assert.Contains("\"BirthDate\": \"15.04.2005\"", json);
        Assert.Contains("\"FirstName\": \"Анна\"", json);
    }

    [Fact]
    public void Serialize_ShouldIgnoreNullValues()
    {
        var student = CreateStudent();
        student.Grades = null!;

        var json = StudentJsonService.Serialize(student);

        Assert.DoesNotContain("\"Grades\"", json);
    }

    [Fact]
    public void Deserialize_ShouldRestoreStudent()
    {
        var json = StudentJsonService.Serialize(CreateStudent());

        var student = StudentJsonService.Deserialize(json);

        Assert.Equal("Анна", student.FirstName);
        Assert.Equal("Иванова", student.LastName);
        Assert.Equal(new DateTime(2005, 4, 15), student.BirthDate);
        Assert.Equal(2, student.Grades.Count);
        Assert.Equal("Математика", student.Grades[0].Name);
        Assert.Equal(5, student.Grades[0].Grade);
    }

    [Fact]
    public void Deserialize_ShouldIgnorePropertyNameCase()
    {
        const string json = """
            {
              "firstname": "Анна",
              "lastname": "Иванова",
              "birthdate": "15.04.2005",
              "grades": []
            }
            """;

        var student = StudentJsonService.Deserialize(json);

        Assert.Equal("Анна", student.FirstName);
    }

    [Fact]
    public void SaveAndLoad_ShouldPreserveStudent()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var filePath = Path.Combine(directory, "student.json");

        try
        {
            StudentJsonService.SaveToFile(CreateStudent(), filePath);
            var student = StudentJsonService.LoadFromFile(filePath);

            Assert.True(File.Exists(filePath));
            Assert.Equal("Анна", student.FirstName);
            Assert.Equal(2, student.Grades.Count);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
    }

    [Fact]
    public void Deserialize_WithEmptyFirstName_ShouldThrowException()
    {
        const string json = """
            {
              "FirstName": "",
              "LastName": "Иванова",
              "BirthDate": "15.04.2005",
              "Grades": []
            }
            """;

        Assert.Throws<InvalidDataException>(() => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void Deserialize_WithEmptyLastName_ShouldThrowException()
    {
        const string json = """
            {
              "FirstName": "Анна",
              "LastName": "",
              "BirthDate": "15.04.2005",
              "Grades": []
            }
            """;

        Assert.Throws<InvalidDataException>(() => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void Deserialize_WithFutureBirthDate_ShouldThrowException()
    {
        var futureDate = DateTime.Today.AddYears(1).ToString("dd.MM.yyyy");
        var json = $$"""
            {
              "FirstName": "Анна",
              "LastName": "Иванова",
              "BirthDate": "{{futureDate}}",
              "Grades": []
            }
            """;

        Assert.Throws<InvalidDataException>(() => StudentJsonService.Deserialize(json));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Deserialize_WithInvalidGrade_ShouldThrowException(int grade)
    {
        var json = $$"""
            {
              "FirstName": "Анна",
              "LastName": "Иванова",
              "BirthDate": "15.04.2005",
              "Grades": [
                { "Name": "Математика", "Grade": {{grade}} }
              ]
            }
            """;

        Assert.Throws<InvalidDataException>(() => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void Deserialize_WithEmptySubjectName_ShouldThrowException()
    {
        const string json = """
            {
              "FirstName": "Анна",
              "LastName": "Иванова",
              "BirthDate": "15.04.2005",
              "Grades": [
                { "Name": "", "Grade": 5 }
              ]
            }
            """;

        Assert.Throws<InvalidDataException>(() => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void Deserialize_WithInvalidDateFormat_ShouldThrowException()
    {
        const string json = """
            {
              "FirstName": "Анна",
              "LastName": "Иванова",
              "BirthDate": "2005-04-15",
              "Grades": []
            }
            """;

        Assert.Throws<JsonException>(() => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void Deserialize_WithMissingGrades_ShouldThrowException()
    {
        const string json = """
            {
              "FirstName": "Анна",
              "LastName": "Иванова",
              "BirthDate": "15.04.2005",
              "Grades": null
            }
            """;

        Assert.Throws<InvalidDataException>(() => StudentJsonService.Deserialize(json));
    }

    [Fact]
    public void Deserialize_WithMalformedJson_ShouldThrowException()
    {
        const string json = "{ invalid json }";

        Assert.Throws<JsonException>(() => StudentJsonService.Deserialize(json));
    }

    private static Student CreateStudent()
    {
        return new Student
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
    }
}
