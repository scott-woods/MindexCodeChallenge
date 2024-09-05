using CodeChallenge.Data;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeRepositoryTests
    {
        private EmployeeContext _employeeContext;
        private EmployeeRespository _employeeRepository;
        private ILogger<IEmployeeRepository> _logger;

        [TestInitialize]
        public void Setup()
        {
            //use in-memory db for testing
            var options = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _employeeContext = new EmployeeContext(options);
            _logger = new LoggerFactory().CreateLogger<IEmployeeRepository>();
            _employeeRepository = new EmployeeRespository(_logger, _employeeContext);
        }

        #region Add Tests

        [TestMethod]
        public void Add_ValidEmployee_ReturnsEmployeeWithId()
        {
            //Arrange
            var employee = new Employee
            {
                FirstName = "Scott",
                LastName = "Woods",
                Position = "Software Engineer",
                Department = "Fun"
            };

            //Execute
            var result = _employeeRepository.Add(employee);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.EmployeeId));
            Assert.AreEqual("Scott", result.FirstName);
        }

        #endregion

        #region GetById Tests

        [TestMethod]
        public void GetById_ValidId_ReturnsEmployee()
        {
            //Arrange
            var employee = new Employee
            {
                EmployeeId = Guid.NewGuid().ToString(),
                FirstName = "Scott",
                LastName = "Woods",
                Position = "Software Engineer",
                Department = "Fun"
            };

            _employeeRepository.Add(employee);
            _employeeRepository.SaveAsync().Wait();

            //Execute
            var result = _employeeRepository.GetById(employee.EmployeeId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(employee.EmployeeId, result.EmployeeId);
            Assert.AreEqual("Scott", result.FirstName);
        }

        [TestMethod]
        public void GetById_InvalidId_ReturnsNull()
        {
            //Arrange
            var invalidId = "invalid-id";

            //Execute
            var result = _employeeRepository.GetById(invalidId);

            //Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Remove Tests

        [TestMethod]
        public void Remove_ValidEmployee_ReturnsRemovedEmployee()
        {
            //Arrange
            var employee = new Employee
            {
                EmployeeId = Guid.NewGuid().ToString(),
                FirstName = "Scott",
                LastName = "Woods",
                Position = "Software Engineer",
                Department = "Fun"
            };

            _employeeRepository.Add(employee);
            _employeeRepository.SaveAsync().Wait();

            //Execute
            var result = _employeeRepository.Remove(employee);
            _employeeRepository.SaveAsync().Wait();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(employee.EmployeeId, result.EmployeeId);
            Assert.IsNull(_employeeRepository.GetById(employee.EmployeeId)); // Ensure the employee was removed
        }

        #endregion

        #region SaveAsync Tests

        [TestMethod]
        public void SaveAsync_AddsToDatabase()
        {
            //Arrange
            var employee = new Employee
            {
                EmployeeId = Guid.NewGuid().ToString(),
                FirstName = "Scott",
                LastName = "Woods",
                Position = "Software Engineer",
                Department = "Fun"
            };

            _employeeRepository.Add(employee);

            //Execute
            _employeeRepository.SaveAsync().Wait();

            //Assert
            var savedEmployee = _employeeContext.Employees.FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);
            Assert.IsNotNull(savedEmployee);
            Assert.AreEqual(employee.FirstName, savedEmployee.FirstName);
        }

        #endregion
    }
}
