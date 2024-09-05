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
    public class CompensationRepositoryTests
    {
        private EmployeeContext _employeeContext;
        private CompensationRepository _compensationRepository;
        private ILogger<ICompensationRepository> _logger;

        [TestInitialize]
        public void Setup()
        {
            //use in-memory db for testing
            var options = new DbContextOptionsBuilder<EmployeeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _employeeContext = new EmployeeContext(options);
            _logger = new LoggerFactory().CreateLogger<ICompensationRepository>();
            _compensationRepository = new CompensationRepository(_logger, _employeeContext);
        }

        #region Add Tests

        [TestMethod]
        public void Add_ValidCompensation_ReturnsCompensationWithId()
        {
            //Arrange
            var employee = new Employee { EmployeeId = "valid-id", FirstName = "Scott", LastName = "Woods" };
            var compensation = new Compensation
            {
                Employee = employee,
                Salary = 100000,
                EffectiveDate = DateTime.Now
            };

            //Execute
            var result = _compensationRepository.Add(compensation);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.CompensationId));
            Assert.AreEqual(compensation.Salary, result.Salary);
            Assert.AreEqual(compensation.Employee.EmployeeId, result.Employee.EmployeeId);
        }

        #endregion

        #region GetByEmployeeId Tests

        [TestMethod]
        public void GetByEmployeeId_ValidEmployeeId_ReturnsCompensation()
        {
            //Arrange
            var employee = new Employee { EmployeeId = "valid-id", FirstName = "Scott", LastName = "Woods" };
            var compensation = new Compensation
            {
                Employee = employee,
                Salary = 100000,
                EffectiveDate = DateTime.Now
            };

            _compensationRepository.Add(compensation);
            _compensationRepository.SaveAsync().Wait();

            //Execute
            var result = _compensationRepository.GetByEmployeeId(employee.EmployeeId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(compensation.Employee.EmployeeId, result.Employee.EmployeeId);
            Assert.AreEqual(compensation.Salary, result.Salary);
        }

        [TestMethod]
        public void GetByEmployeeId_InvalidEmployeeId_ReturnsNull()
        {
            //Arrange
            var invalidEmployeeId = "invalid-id";

            //Execute
            var result = _compensationRepository.GetByEmployeeId(invalidEmployeeId);

            //Assert
            Assert.IsNull(result);
        }

        #endregion

        #region SaveAsync Tests

        [TestMethod]
        public void SaveAsync_AddsCompensationToDatabase()
        {
            //Arrange
            var employee = new Employee { EmployeeId = "valid-id", FirstName = "Scott", LastName = "Woods" };
            var compensation = new Compensation
            {
                Employee = employee,
                Salary = 100000,
                EffectiveDate = DateTime.Now
            };

            _compensationRepository.Add(compensation);

            //Execute
            _compensationRepository.SaveAsync().Wait();

            //Assert
            var savedCompensation = _employeeContext.Compensations.FirstOrDefault(c => c.Employee.EmployeeId == employee.EmployeeId);
            Assert.IsNotNull(savedCompensation);
            Assert.AreEqual(compensation.Salary, savedCompensation.Salary);
            Assert.AreEqual(compensation.Employee.EmployeeId, savedCompensation.Employee.EmployeeId);
        }

        #endregion
    }
}
