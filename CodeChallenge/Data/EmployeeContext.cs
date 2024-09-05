using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data
{
    /// <summary>
    /// Employee db context
    /// </summary>
    public class EmployeeContext : DbContext
    {
        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="options"></param>
        public EmployeeContext(DbContextOptions<EmployeeContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.DirectReports);
        }

        /// <summary>
        /// Employees db set
        /// </summary>
        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// Compensations db set
        /// </summary>
        public DbSet<Compensation> Compensations { get; set; }
    }
}
