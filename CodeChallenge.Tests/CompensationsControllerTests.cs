using CodeChallenge.Controllers;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
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
    public class CompensationsControllerTests
    {
        private Mock<ICompensationService> _compensationServiceMock;
        private Mock<ILogger<CompensationsController>> _loggerMock;
        private CompensationsController _compensationsController;

        [TestInitialize]
        public void Setup()
        {
            _compensationServiceMock = new Mock<ICompensationService>();
            _loggerMock = new Mock<ILogger<CompensationsController>>();
            _compensationsController = new CompensationsController(_loggerMock.Object, _compensationServiceMock.Object);
        }

        #region GetCompensationByEmployeeId Tests

        [TestMethod]
        public void GetCompensationByEmployeeId_ValidId_ReturnsOk()
        {
            //Arrange
            var employeeId = "valid-id";
            var compensation = new Compensation
            {
                EmployeeId = employeeId,
                Salary = 50000
            };
            _compensationServiceMock.Setup(s => s.GetByEmployeeId(employeeId))
                                    .Returns(compensation);

            //Execute
            var result = _compensationsController.GetCompensationByEmployeeId(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(compensation, okResult.Value);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_EmployeeNotFound_ReturnsNotFound()
        {
            //Arrange
            var employeeId = "invalid-id";
            _compensationServiceMock.Setup(s => s.GetByEmployeeId(employeeId))
                                    .Returns((Compensation)null);

            //Execute
            var result = _compensationsController.GetCompensationByEmployeeId(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_ServiceThrowsArgumentException_ReturnsNotFound()
        {
            //Arrange
            var employeeId = "valid-id";
            _compensationServiceMock.Setup(s => s.GetByEmployeeId(employeeId))
                                    .Throws(new ArgumentException("Employee not found"));

            //Execute
            var result = _compensationsController.GetCompensationByEmployeeId(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_ServiceThrowsException_ReturnsInternalServerError()
        {
            //Arrange
            var employeeId = "valid-id";
            _compensationServiceMock.Setup(s => s.GetByEmployeeId(employeeId))
                                    .Throws(new Exception("Service error"));

            //Execute
            var result = _compensationsController.GetCompensationByEmployeeId(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        #endregion

        #region CreateCompensation Tests

        [TestMethod]
        public void CreateCompensation_ValidCompensation_ReturnsCreated()
        {
            //Arrange
            var compensationDto = new CompensationDto
            {
                EmployeeId = "valid-id",
                Salary = 50000
            };

            var createdCompensation = new Compensation
            {
                EmployeeId = compensationDto.EmployeeId,
                Salary = compensationDto.Salary
            };

            _compensationServiceMock.Setup(s => s.Create(compensationDto))
                                    .Returns(createdCompensation);

            //Execute
            var result = _compensationsController.CreateCompensation(compensationDto);

            //Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteResult));
            var createdResult = result as CreatedAtRouteResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual("getCompensationByEmployeeId", createdResult.RouteName);
            Assert.AreEqual(compensationDto.EmployeeId, createdResult.RouteValues["employeeId"]);
            Assert.AreEqual(createdCompensation, createdResult.Value);
        }

        [TestMethod]
        public void CreateCompensation_NullCompensation_ReturnsBadRequest()
        {
            //Arrange
            CompensationDto compensationDto = null;

            //Execute
            var result = _compensationsController.CreateCompensation(compensationDto);

            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_InvalidSalary_ReturnsBadRequest()
        {
            //Arrange
            var compensationDto = new CompensationDto
            {
                EmployeeId = "valid-id",
                Salary = -50000
            };

            //Execute
            var result = _compensationsController.CreateCompensation(compensationDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_EmployeeNotFound_ReturnsNotFound()
        {
            //Arrange
            var compensationDto = new CompensationDto
            {
                EmployeeId = "invalid-id",
                Salary = 50000
            };

            _compensationServiceMock.Setup(s => s.Create(compensationDto))
                                    .Throws(new ArgumentException("Employee not found"));

            //Execute
            var result = _compensationsController.CreateCompensation(compensationDto);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_CompensationConflict_ReturnsConflict()
        {
            //Arrange
            var compensationDto = new CompensationDto
            {
                EmployeeId = "valid-id",
                Salary = 50000
            };

            _compensationServiceMock.Setup(s => s.Create(compensationDto))
                                    .Throws(new InvalidOperationException("Compensation already exists"));

            //Execute
            var result = _compensationsController.CreateCompensation(compensationDto);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));
            var conflictResult = result as ConflictObjectResult;
            Assert.IsNotNull(conflictResult);
            Assert.AreEqual(409, conflictResult.StatusCode);
        }

        #endregion
    }
}
