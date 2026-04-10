using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.Services;
using StudentManagementAPI.Models;
using System.Linq;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllStudents();

            var result = data.Select(x => new
            {
                x.Id,
                x.Name,
                x.Email,
                x.Age,
                x.Course,
                CreatedDate = x.CreatedDate.ToString("dd-MM-yyyy HH:mm")
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _service.GetStudentById(id);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Student student)
        {
            await _service.AddStudent(student);
            return Ok("Added");
        }

        [HttpPut]
        public async Task<IActionResult> Update(Student student)

        {
            await _service.UpdateStudent(student);
            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteStudent(id);
            return Ok("Deleted");
        }
    }
}