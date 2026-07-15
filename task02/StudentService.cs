namespace task02;

public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students) => _students = students;

    // студенты указанного факультета
    public IEnumerable<Student> GetStudentsByFaculty(string faculty) =>
        _students.Where(student => student.Faculty == faculty);

    // студенты с нужным средним баллом
    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade) =>
        _students.Where(student => student.Grades.Average() >= minAverageGrade);

    // сортировка студентов по имени
    public IEnumerable<Student> GetStudentsOrderedByName() =>
        _students.OrderBy(student => student.Name);

    // группировка студентов по факультету
    public ILookup<string, Student> GroupStudentsByFaculty() =>
        _students.ToLookup(student => student.Faculty);

    // факультет с самым высоким средним баллом
    public string GetFacultyWithHighestAverageGrade() =>
        _students
            .GroupBy(student => student.Faculty)
            .OrderByDescending(group => group
                .SelectMany(student => student.Grades)
                .Average())
            .First()
            .Key;
}
