using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelAuthorization;
using WebApplication1.Domain;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDbContext _context;
        private readonly ICrudAuthorizationPolicyProvider _policyProvider;

        public StudentController(StudentDbContext context, ICrudAuthorizationPolicyProvider policyProvider)
        {
            _context = context;
            _policyProvider = policyProvider;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] StudentModel student)
        {
            await _context.Students
                .AddAsync(student);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id)
        {
            StudentModel? student = await _context.Students.SingleOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            List<StudentModel> students = await _context.Students.ToListAsync();

            if (students.Count == 0)
            {
                return NoContent();
            }

            return Ok(students);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            StudentModel? student = await _context.Students.SingleOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            return Ok(student);
        }
    }
}
