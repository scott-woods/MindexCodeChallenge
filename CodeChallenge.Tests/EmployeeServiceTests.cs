using CodeChallenge.Models;
using CodeChallenge.Repositories;
using CodeChallenge.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<ILogger<EmployeeService>> _loggerMock;
        private EmployeeService _employeeService;

        [TestInitialize]
        public void Setup()
        {
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _loggerMock = new Mock<ILogger<EmployeeService>>();
            _employeeService = new EmployeeService(_loggerMock.Object, _employeeRepositoryMock.Object);
        }

        #region Create Tests

        [TestMethod]
        public void Create_ValidEmployeeDto_ReturnsEmployee()
        {
            //Arrange
            var employeeDto = new EmployeeDto
            {
                FirstName = "Scott",
                LastName = "Woods",
                Position = "Software Engineer",
                Department = "Fun"
            };

            var employee = new Employee
            {
                EmployeeId = "1",
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Position = employeeDto.Position,
                Department = employeeDto.Department
            };

            _employeeRepositoryMock.Setup(r => r.Add(It.IsAny<Employee>()));
            _employeeRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            //Execute
            var result = _employeeService.Create(employeeDto);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(employeeDto.FirstName, result.FirstName);
            Assert.AreEqual(employeeDto.LastName, result.LastName);
            Assert.AreEqual(employeeDto.Position, result.Position);
        }

        #endregion

        #region GetById Tests

        [TestMethod]
        public void GetById_ValidId_ReturnsEmployee()
        {
            //Arrange
            var employeeId = "1";
            var employee = new Employee { EmployeeId = employeeId, FirstName = "Scott", LastName = "Woods" };

            _employeeRepositoryMock.Setup(r => r.GetById(employeeId)).Returns(employee);

            //Execute
            var result = _employeeService.GetById(employeeId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(employeeId, result.EmployeeId);
            Assert.AreEqual("Scott", result.FirstName);
        }

        [TestMethod]
        public void GetById_InvalidId_ReturnsNull()
        {
            //Arrange
            var employeeId = "invalid-id";

            _employeeRepositoryMock.Setup(r => r.GetById(employeeId)).Returns((Employee)null);

            //Execute
            var result = _employeeService.GetById(employeeId);

            //Assert
            Assert.IsNull(result);
        }

        #endregion

        #region GetReportingStructure Tests

        [TestMethod]
        public void GetReportingStructure_ValidEmployeeId_ReturnsReportingStructure()
        {
            //Arrange
            var employeeId = "1";
            var employee = new Employee
            {
                EmployeeId = employeeId,
                DirectReports = new List<Employee> { new Employee { EmployeeId = "2" } }
            };

            _employeeRepositoryMock.Setup(r => r.GetById(employeeId)).Returns(employee);
            _employeeRepositoryMock.Setup(r => r.GetById("2")).Returns(new Employee { EmployeeId = "2" });

            //Execute
            var result = _employeeService.GetReportingStructure(employeeId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.NumberOfReports);
            Assert.AreEqual(employee, result.Employee);
        }

        [TestMethod]
        public void GetReportingStructure_InvalidEmployeeId_ReturnsNull()
        {
            //Arrange
            var employeeId = "invalid-id";

            _employeeRepositoryMock.Setup(r => r.GetById(employeeId)).Returns((Employee)null);

            //Execute
            var result = _employeeService.GetReportingStructure(employeeId);

            //Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Replace Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Replace_InvalidOriginalEmployeeId_ThrowsArgumentException()
        {
            //Arrange
            var originalEmployeeId = "invalid-id";
            var newEmployeeDto = new EmployeeDto { FirstName = "Scott", LastName = "Woods" };

            _employeeRepositoryMock.Setup(r => r.GetById(originalEmployeeId)).Returns((Employee)null);

            //Execute
            _employeeService.Replace(originalEmployeeId, newEmployeeDto);

            //Assert is handled by ExpectedException
        }

        [TestMethod]
        public void Replace_ValidOriginalEmployeeId_ReplacesEmployee()
        {
            //Arrange
            var originalEmployeeId = "1";
            var originalEmployee = new Employee { EmployeeId = originalEmployeeId, FirstName = "John", LastName = "Lennon" };
            var newEmployeeDto = new EmployeeDto { FirstName = "Scott", LastName = "Woods", Position = "Software Engineer", Department = "Fun" };

            _employeeRepositoryMock.Setup(r => r.GetById(originalEmployeeId)).Returns(originalEmployee);
            _employeeRepositoryMock.Setup(r => r.Remove(originalEmployee));
            _employeeRepositoryMock.Setup(r => r.Add(It.IsAny<Employee>()));
            _employeeRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            //Execute
            var result = _employeeService.Replace(originalEmployeeId, newEmployeeDto);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(newEmployeeDto.FirstName, result.FirstName);
            Assert.AreEqual(newEmployeeDto.LastName, result.LastName);
            Assert.AreEqual(newEmployeeDto.Position, result.Position);
        }

        #endregion

    }
}