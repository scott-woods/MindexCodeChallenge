using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Services
{
    /// <summary>
    /// service interface for Compensations
    /// </summary>
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<CompensationService> _logger;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="compensationRepository"></param>
        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
        {
            _compensationRepository = compensationRepository;
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        /// <summary>
        /// create a new compensation
        /// </summary>
        /// <param name="compensationDto"></param>
        /// <returns></returns>
        public Compensation Create(CompensationDto compensationDto)
        {
            //validate employee exists
            var employee = _employeeRepository.GetById(compensationDto.EmployeeId);
            if (employee == null)
                throw new ArgumentException($"Employee with Id {compensationDto.EmployeeId} not found.");

            //check that an existing compensation doesn't exist for this employee
            var existingCompensation = _compensationRepository.GetByEmployeeId(compensationDto.EmployeeId);
            if (existingCompensation != null)
                throw new InvalidOperationException($"Compensation already exists for Employee with Id {compensationDto.EmployeeId}");

            //create compensation entity
            var compensation = new Compensation()
            {
                EmployeeId = compensationDto.EmployeeId,
                Salary = compensationDto.Salary,
                EffectiveDate = compensationDto.EffectiveDate
            };

            //save to db
            _compensationRepository.Add(compensation);
            _compensationRepository.SaveAsync().Wait();

            return compensation;
        }

        /// <summary>
        /// get a compensation by employee id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public Compensation GetByEmployeeId(string employeeId)
        {
            //validate employee exists
            var employee = _employeeRepository.GetById(employeeId);
            if (employee == null)
                throw new ArgumentException($"Employee with Id {employeeId} not found.");

            return _compensationRepository.GetByEmployeeId(employeeId);
        }
    }
}
