using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    /// <summary>
    /// Repository for Compensations
    /// </summary>
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _context;
        private readonly ILogger<ICompensationRepository> _logger;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// add a compensation to the database
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            _context.Compensations.Add(compensation);
            return compensation;
        }

        /// <summary>
        /// retrieve a compensation by employee id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public Compensation GetByEmployeeId(string employeeId)
        {
            return _context.Compensations
                .FirstOrDefault(c => c.Employee.EmployeeId == employeeId);
        }

        /// <summary>
        /// save changes to the database
        /// </summary>
        /// <returns></returns>
        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
