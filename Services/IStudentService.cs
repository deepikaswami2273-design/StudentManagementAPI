using StudentManagementAPI.Models;

namespace StudentManagementAPI.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllStudents();
        Task<Student?> GetStudentById(int id);
        Task AddStudent(Student student);
        Task UpdateStudent(Student student);
        Task DeleteStudent(int id);
    }
}