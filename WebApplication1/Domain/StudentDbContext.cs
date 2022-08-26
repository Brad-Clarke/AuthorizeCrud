using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Domain
{
    public class StudentDbContext : DbContext
    {
        public DbSet<StudentModel> Students { get; set; }

        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        {
        }
    }
}
