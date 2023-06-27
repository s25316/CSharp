using Lesson3.Models.DTO;
using Lesson3.Models;
using Lesson3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lesson3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDb _students;

        public StudentsController(IStudentsDb students)
        {
            _students = students;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            StatementListStudentsDTO students = await _students.GetAllStudents();
            return Ok(students);
        }

        [HttpGet("IndexNumber")]
        public async Task<IActionResult> GetStudentsAsync(int IndexNumber)
        {
            StatementStudentDTO student = await _students.GetStudentAsync(IndexNumber);
            return Ok(student);
        }

        [HttpPut("IndexNumber")]
        public async Task<IActionResult> PutStudentsAsync(int IndexNumber, StudentPutDTO studentDTO)
        {
            StatementStudentDTO student = await _students.PutStudentAsync(IndexNumber, studentDTO);
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> PostStudentsAsync(Student s)
        {
            StatementStudentDTO student = await _students.PostStudentAsync(s);
            return Ok(student);
        }

        [HttpDelete("IndexNumber")]
        public async Task<IActionResult> DeleteStudentsAsync(int IndexNumber)
        {
            StatementStudentDTO student = await _students.DeleteStudentAsync(IndexNumber);
            return Ok(student);
        }

    }
}
