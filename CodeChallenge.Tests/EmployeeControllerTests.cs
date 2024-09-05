
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Castle.Core.Logging;
using CodeChallenge.Controllers;
using CodeChallenge.Models;
using CodeChallenge.Services;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        private Mock<IEmployeeService> _employeeServiceMock;
        private Mock<ILogger<EmployeeController>> _loggerMock;
        private EmployeeController _employeeController;

        [TestInitialize]
        public void Setup()
        {
            //create mock services
            _employeeServiceMock = new Mock<IEmployeeService>();
            _loggerMock = new Mock<ILogger<EmployeeController>>();

            //init employee controller
            _employeeController = new EmployeeController(_loggerMock.Object, _employeeServiceMock.Object);
        }

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #region GetReportingStructureById Tests

        [TestMethod]
        public void GetReportingStructureById_ValidId_ReturnsOk()
        {
            //Arrange
            var employeeId = "valid-id";
            var reportingStructure = new ReportingStructure
            {
                Employee = new Employee
                {
                    EmployeeId = employeeId,
                    FirstName = "Scott",
                    LastName = "Woods",
                    Department = "Fun",
                    Position = "Software Engineer"
                },
                NumberOfReports = 2
            };

            _employeeServiceMock.Setup(s => s.GetReportingStructure(employeeId)).Returns(reportingStructure);

            //Execute
            var result = _employeeController.GetReportingStructureById(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(okResult.StatusCode, (int)HttpStatusCode.OK);
            Assert.AreEqual(reportingStructure, okResult.Value);
        }

        [TestMethod]
        public void GetReportingStructureById_EmptyId_ReturnsBadRequest()
        {
            //Arrange
            string employeeId = "";

            //Execute
            var result = _employeeController.GetReportingStructureById(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }

        [TestMethod]
        public void GetReportingStructureById_EmployeeNotFound_ReturnsNotFound()
        {
            //Arrange
            var employeeId = "invalid-id";

            //mock the service to return null (employee not found)
            _employeeServiceMock.Setup(s => s.GetReportingStructure(employeeId))
                                .Returns((ReportingStructure)null);

            //Execute
            var result = _employeeController.GetReportingStructureById(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        public void GetReportingStructureById_ServiceThrowsException_ReturnsInternalServerError()
        {
            //Arrange
            var employeeId = "valid-id";

            //mock the service to throw an exception
            _employeeServiceMock.Setup(s => s.GetReportingStructure(employeeId))
                                .Throws(new Exception("Service failed"));

            //Execute
            var result = _employeeController.GetReportingStructureById(employeeId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var serverErrorResult = result as ObjectResult;
            Assert.IsNotNull(serverErrorResult);
            Assert.AreEqual(500, serverErrorResult.StatusCode);
        }

        #endregion
    }
}
