using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Controllers
{
    /// <summary>
    /// Controller for managing employees
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="employeeService"></param>
        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        #region GET

        /// <summary>
        /// Get an employee by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogInformation($"Received employee get request for '{id}'");

            try
            {
                //validate id
                if (String.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Id is required");
                    return BadRequest("Id is required");
                }

                //retrieve employee by id
                var employee = _employeeService.GetById(id);

                if (employee == null)
                {
                    _logger.LogWarning($"Employee with Id {id} not found.");
                    return NotFound($"Employee with Id {id} not found.");
                }

                _logger.LogInformation(id, $"Employee with Id {id} successfully retrieved.");

                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while trying to retrieve Employee.");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get the reporting structure for an employee by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ReportingStructure), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("reportingStructure/{id}", Name = "getReportingStructureById")]
        public IActionResult GetReportingStructureById(String id)
        {
            _logger.LogInformation($"Received employee reporting structure get request for '{id}'");

            try
            {
                //validate id
                if (String.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Id is required.");
                    return BadRequest("Id is required");
                }

                //retrieve reporting structure
                var reportingStructure = _employeeService.GetReportingStructure(id);

                //if null, employee was not found
                if (reportingStructure == null)
                {
                    _logger.LogWarning($"Employee with Id {id} not found.");
                    return NotFound($"Employee with Id {id} not found.");
                }

                _logger.LogInformation($"Reporting Structure for Employee with Id {id} successfully retrieved.");

                return Ok(reportingStructure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while trying to retrieve Reporting Structure.");
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// Create an employee
        /// </summary>
        /// <param name="employeeDto"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Employee), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] EmployeeDto employeeDto)
        {
            _logger.LogInformation($"Received employee create request for '{employeeDto.FirstName} {employeeDto.LastName}'");

            try
            {
                //add employee to db
                var createdEmployee = _employeeService.Create(employeeDto);

                _logger.LogInformation("Employee successfully created and saved.");

                return CreatedAtRoute("getEmployeeById", new { id = createdEmployee.EmployeeId }, createdEmployee);
            }
            catch (ArgumentException ex) //handle direct report not found
            {
                _logger.LogError(ex, "Exception occurred while creating Employee.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating Employee.");
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region PUT

        /// <summary>
        /// Replace an employee by id with a new employee
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newEmployeeDto"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Employee), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody] EmployeeDto newEmployeeDto)
        {
            _logger.LogInformation($"Recieved employee update request for '{id}'");

            try
            {
                //validate id
                if (String.IsNullOrEmpty(id))
                {
                    _logger.LogWarning("Id is required");
                    return BadRequest("Id is required");
                }

                //replace employee
                var newEmployee = _employeeService.Replace(id, newEmployeeDto);

                _logger.LogInformation($"Employee with Id {id} successfully replaced.");

                return Ok(newEmployee);
            }
            catch (ArgumentException ex) //handle not found errors
            {
                _logger.LogError(ex, "Exception occurred while updating Employee.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating Employee.");
                return StatusCode(500, ex.Message);
            }
        }

        #endregion
    }
}
