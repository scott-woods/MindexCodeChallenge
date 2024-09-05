using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    /// <summary>
    /// Employee service
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        #region CREATE

        /// <summary>
        /// create an employee
        /// </summary>
        /// <param name="employeeDto"></param>
        /// <returns></returns>
        public Employee Create(EmployeeDto employeeDto)
        {
            //convert to employee entity
            var employee = MapDtoToEmployee(employeeDto);

            //add employee to db
            _employeeRepository.Add(employee);
            _employeeRepository.SaveAsync().Wait();

            return employee;
        }

        #endregion

        #region READ

        /// <summary>
        /// get an employee by their id. returns null if employee is not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        /// <summary>
        /// given an employee id, retrieve the employee and calculate the total number of reports for the employee. returns null if employee is not found
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public ReportingStructure GetReportingStructure(String employeeId)
        {
            //retrieve the employee
            var employee = GetById(employeeId);

            //if employee is null, break early
            if (employee == null)
                return null;

            //calculate total number of reports for this employee
            var numberOfReports = CalculateTotalReports(employee);

            //return ReportingStructure object
            return new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = numberOfReports
            };
        }

        #endregion

        #region UPDATE

        /// <summary>
        /// replace an employee by id with a new employee entity
        /// </summary>
        /// <param name="originalEmployeeId"></param>
        /// <param name="newEmployeeDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Employee Replace(String originalEmployeeId, EmployeeDto newEmployeeDto)
        {
            //get the original employee
            var originalEmployee = GetById(originalEmployeeId);

            //validate original employee exists
            if (originalEmployee == null)
                throw new ArgumentException($"Employee with Id {originalEmployeeId} not found.");

            //remove original employee
            _employeeRepository.Remove(originalEmployee);

            //save and wait until removal is complete
            _employeeRepository.SaveAsync().Wait();

            try
            {
                //convert dto to employee entity
                var employee = MapDtoToEmployee(newEmployeeDto);
                employee.EmployeeId = originalEmployeeId;

                //add employee entity to db
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();

                return employee;
            }
            catch (Exception ex)
            {
                //if replacement fails, restore original employee (normally use a transaction for this, but not supported by in-memory db)
                _employeeRepository.Add(originalEmployee);
                _employeeRepository.SaveAsync().Wait();

                throw;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Calculate the total number of reports for an employee
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        private int CalculateTotalReports(Employee employee)
        {
            //if employee is null, or has no direct reports, return 0
            if (employee == null || employee.DirectReports == null || employee.DirectReports.Count == 0)
                return 0;

            //recursively calculate the total number of reports for the employee
            int totalReports = employee.DirectReports.Count;
            foreach (var directReport in employee.DirectReports)
            {
                var nextEmployee = GetById(directReport.EmployeeId);
                totalReports += CalculateTotalReports(nextEmployee);
            }

            return totalReports;
        }

        /// <summary>
        /// converts an EmployeeDto to an Employee entity
        /// </summary>
        /// <param name="employeeDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        Employee MapDtoToEmployee(EmployeeDto employeeDto)
        {
            //create employee entity
            var employee = new Employee()
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Position = employeeDto.Position,
                Department = employeeDto.Department
            };

            //handle direct reports
            if (employeeDto.DirectReportIds != null && employeeDto.DirectReportIds.Count > 0)
            {
                foreach (var id in employeeDto.DirectReportIds)
                {
                    //get direct report by id
                    var directReport = GetById(id);

                    //if provided with a direct report that doesn't exist, don't add the employee
                    if (directReport == null)
                        throw new ArgumentException($"Direct Report with Id {id} not found.");

                    //add direct report to employee entity
                    employee.DirectReports.Add(directReport);
                }
            }

            return employee;
        }

        #endregion
    }
}
