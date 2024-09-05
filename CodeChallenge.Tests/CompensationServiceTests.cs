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
    public class CompensationServiceTests
    {
        private Mock<ICompensationRepository> _compensationRepositoryMock;
        private Mock<IEmployeeRepository> _employeeRepositoryMock;
        private Mock<ILogger<CompensationService>> _loggerMock;
        private CompensationService _compensationService;

        [TestInitialize]
        public void Setup()
        {
            _compensationRepositoryMock = new Mock<ICompensationRepository>();
            _employeeRepositoryMock = new Mock<IEmployeeRepository>();
            _loggerMock = new Mock<ILogger<CompensationService>>();
            _compensationService = new CompensationService(_loggerMock.Object, _compensationRepositoryMock.Object, _employeeRepositoryMock.Object);
        }

        #region Create Tests

        [TestMethod]
        public void Create_ValidCompensationDto_ReturnsCompensation()
        {
            //Arrange
            var compensationDto = new CompensationDto
            {
                EmployeeId = "valid-id",
                Salary = 100000,
                EffectiveDate = DateTime.Now
            };

            var employee = new Employee { EmployeeId = compensationDto.EmployeeId };

            _employeeRepositoryMock.Setup(r => r.GetById(compensationDto.EmployeeId)).Returns(employee);
            _compensationRepositoryMock.Setup(r => r.GetByEmployeeId(compensationDto.EmployeeId)).Returns((Compensation)null);
            _compensationRepositoryMock.Setup(r => r.Add(It.IsAny<Compensation>()));
            _compensationRepositoryMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

            //Execute
            var result = _compensationService.Create(compensationDto);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(compensationDto.EmployeeId, result.EmployeeId);
            Assert.AreEqual(compensationDto.Salary, result.Salary);
            Assert.AreEqual(compensationDto.EffectiveDate, result.EffectiveDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Create_EmployeeNotFound_ThrowsArgumentException()
        {
            //Arrange
            var compensationDto = new CompensationDto
            {
                EmployeeId = "invalid-id",
                Salary = 100000,
                EffectiveDate = DateTime.Now
            };

            _employeeRepositoryMock.Setup(r => r.GetById(compensationDto.EmployeeId)).Returns((Employee)null);

            //Execute
            _compensationService.Create(compensationDto);

            //Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Create_CompensationAlreadyExists_ThrowsInvalidOperationException()
        {
            //Arrange
            var compensationDto = new CompensationDto
            {
                EmployeeId = "valid-id",
                Salary = 100000,
                EffectiveDate = DateTime.Now
            };

            var employee = new Employee { EmployeeId = compensationDto.EmployeeId };
            var existingCompensation = new Compensation { EmployeeId = compensationDto.EmployeeId, Salary = 90000 };

            _employeeRepositoryMock.Setup(r => r.GetById(compensationDto.EmployeeId)).Returns(employee);
            _compensationRepositoryMock.Setup(r => r.GetByEmployeeId(compensationDto.EmployeeId)).Returns(existingCompensation);

            //Execute
            _compensationService.Create(compensationDto);

            //Assert is handled by ExpectedException
        }

        #endregion

        #region GetByEmployeeId Tests

        [TestMethod]
        public void GetByEmployeeId_ValidId_ReturnsCompensation()
        {
            //Arrange
            var employeeId = "valid-id";
            var employee = new Employee { EmployeeId = employeeId };
            var compensation = new Compensation { EmployeeId = employeeId, Salary = 100000 };

            _employeeRepositoryMock.Setup(r => r.GetById(employeeId)).Returns(employee);
            _compensationRepositoryMock.Setup(r => r.GetByEmployeeId(employeeId)).Returns(compensation);

            //Execute
            var result = _compensationService.GetByEmployeeId(employeeId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(employeeId, result.EmployeeId);
            Assert.AreEqual(100000, result.Salary);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetByEmployeeId_EmployeeNotFound_ThrowsArgumentException()
        {
            //Arrange
            var employeeId = "invalid-id";

            _employeeRepositoryMock.Setup(r => r.GetById(employeeId)).Returns((Employee)null);

            //Execute
            _compensationService.GetByEmployeeId(employeeId);

            //Assert is handled by ExpectedException
        }

        [TestMethod]
        public void GetByEmployeeId_CompensationNotFound_ReturnsNull()
        {
            //Arrange
            var employeeId = "valid-id";
            var employee = new Employee { EmployeeId = employeeId };

            _employeeRepositoryMock.Setup(r => r.GetById(employeeId)).Returns(employee);
            _compensationRepositoryMock.Setup(r => r.GetByEmployeeId(employeeId)).Returns((Compensation)null);

            //Execute
            var result = _compensationService.GetByEmployeeId(employeeId);

            //Assert
            Assert.IsNull(result);
        }

        #endregion
    }

}
