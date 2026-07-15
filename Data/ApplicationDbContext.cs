using Microsoft.EntityFrameworkCore;
using TmsApi.Models;


namespace TmsApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }

    public class Enrollment
    {
        public int Id { get; set; }
        public required string StudentId { get; set; }
        public required string CourseCode { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }
        public required string StudentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
