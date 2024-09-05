using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    /// <summary>
    /// Employee service interface
    /// </summary>
    public interface IEmployeeService
    {
        Employee GetById(String id);
        Employee Create(EmployeeDto employeeDto);
        Employee Replace(String originalEmployeeId, EmployeeDto newEmployeeDto);
        ReportingStructure GetReportingStructure(String employeeId);
    }
}
