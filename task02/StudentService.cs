using System;
using System.Collections.Generic;
using System.Linq;

namespace task02;

public class StudentService
{
    private readonly List<Student> _students;

    public StudentService(List<Student> students) => _students = students;

    public IEnumerable<Student> GetStudentsByFaculty(string faculty)
    {
        return _students.Where(student => student.Faculty.Equals(faculty));
    }

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade)
    {
        return _students.Where(student => student.Grades.Average() >= minAverageGrade);
    }

    public IEnumerable<Student> GetStudentsOrderedByName()
    {
        return _students.OrderBy(student => student.Name);
    }

    public ILookup<string, Student> GroupStudentsByFaculty()
    {
        return _students.ToLookup(student => student.Faculty);
    }

    public string GetFacultyWithHighestAverageGrade()
    {
        var facultyAverages = _students
            .GroupBy(student => student.Faculty)
            .Select(group => new
            {
                FacultyName = group.Key,
                AverageScore = group.Average(student => student.Grades.Average())
            });

        return facultyAverages
            .OrderByDescending(f => f.AverageScore)
            .First()
            .FacultyName;
    }
}