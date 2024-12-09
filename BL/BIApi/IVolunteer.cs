namespace BIApi;
public interface IVolunteer
{
    void Create(BO.Student boStudent);
    BO.Student? Read(int id);

    IEnumerable<BO.StudentInList> ReadAll(BO.StudentFieldSort? sort = null, BO.StudentFieldFilter? filter = null, object? value = null);
    void Update(BO.Student boStudent);
    void Delete(int id);

    void RegisterStudentToCourse(int studentId, int courseId);
    void UnRegisterStudentFromCourse(int studentId, int courseId);

    IEnumerable<BO.CourseInList> GetRegisteredCoursesForStudent(int studentId, BO.Year year = BO.Year.None);
    IEnumerable<BO.CourseInList> GetUnRegisteredCoursesForStudent(int studentId, BO.Year year = BO.Year.None);

    BO.StudentGradeSheet GetGradeSheetPerStudent(int studentId, BO.Year year = BO.Year.None);
    void UpdateGrade(int studentId, int courseId, double grade);

}
