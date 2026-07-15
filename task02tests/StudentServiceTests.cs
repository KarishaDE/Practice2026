using task02;

namespace task02tests;

[TestFixture]
public class StudentServiceTests
{
    private List<Student> _testStudents = null!;
    private StudentService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _testStudents = new List<Student>
        {
            new() { Name = "Иван", Faculty = "ФИТ", Grades = new List<int> { 5, 4, 5 } },
            new() { Name = "Анна", Faculty = "ФИТ", Grades = new List<int> { 3, 4, 3 } },
            new() { Name = "Петр", Faculty = "Экономика", Grades = new List<int> { 5, 5, 5 } }
        };

        _service = new StudentService(_testStudents);
    }

    [Test]
    public void GetStudentsByFaculty_ReturnsCorrectStudents()
    {
        var result = _service.GetStudentsByFaculty("ФИТ").ToList();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(student => student.Faculty == "ФИТ"), Is.True);
    }

    [Test]
    public void GetStudentsWithMinAverageGrade_ReturnsCorrectStudents()
    {
        var result = _service.GetStudentsWithMinAverageGrade(4).ToList();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(student => student.Grades.Average() >= 4), Is.True);
    }

    [Test]
    public void GetStudentsOrderedByName_ReturnsStudentsInAlphabeticalOrder()
    {
        var result = _service.GetStudentsOrderedByName().ToList();

        Assert.That(result[0].Name, Is.EqualTo("Анна"));
        Assert.That(result[1].Name, Is.EqualTo("Иван"));
        Assert.That(result[2].Name, Is.EqualTo("Петр"));
    }

    [Test]
    public void GroupStudentsByFaculty_ReturnsCorrectGroups()
    {
        var result = _service.GroupStudentsByFaculty();

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result["ФИТ"].Count(), Is.EqualTo(2));
        Assert.That(result["Экономика"].Count(), Is.EqualTo(1));
    }

    [Test]
    public void GetFacultyWithHighestAverageGrade_ReturnsCorrectFaculty()
    {
        var result = _service.GetFacultyWithHighestAverageGrade();

        Assert.That(result, Is.EqualTo("Экономика"));
    }
}
