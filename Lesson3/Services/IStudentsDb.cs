using Lesson3.Models;
using Lesson3.Models.DTO;

namespace Lesson3.Services
{
    public interface IStudentsDb
    {
        Task<StatementListStudentsDTO> GetAllStudents();
        Task<StatementStudentDTO> GetStudentAsync(int IndexNumber);
        Task<StatementStudentDTO> PutStudentAsync(int IndexNumber, StudentPutDTO studentPutDTO);
        Task<StatementStudentDTO> PostStudentAsync(Student s);
        Task<StatementStudentDTO> DeleteStudentAsync(int IndexNumber);
    }
}
