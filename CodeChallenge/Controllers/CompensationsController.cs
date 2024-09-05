using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CodeChallenge.Controllers
{
    /// <summary>
    /// Controller for managing Compensations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CompensationsController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="compensationService"></param>
        public CompensationsController(ILogger<CompensationsController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        #region GET

        /// <summary>
        /// Retrieve compensation by employee id
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Compensation), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet("{employeeId}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(string employeeId)
        {
            _logger.LogInformation($"Received compensation get request for '{employeeId}'");

            try
            {
                //retrieve compensation by employee id
                var compensation = _compensationService.GetByEmployeeId(employeeId);

                //if compensation is null, return not found
                if (compensation == null)
                {
                    var msg = $"Compensation with Employee Id {employeeId} not found.";
                    _logger.LogWarning(msg);
                    return NotFound(msg);
                }

                _logger.LogInformation($"Compensation retrieved for Employee Id {employeeId}.");

                //return compensation
                return Ok(compensation);
            }
            catch (ArgumentException ex) //handle employee not being found
            {
                _logger.LogError(ex, $"Exception occured while retrieving Compensation for Employee Id {employeeId}");
                return NotFound(ex.Message);
            }
            catch (Exception ex) //handle other exceptions
            {
                _logger.LogError(ex, $"Exception occured while retrieving Compensation for Employee Id {employeeId}");
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// Create a new compensation
        /// </summary>
        /// <param name="compensation"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Compensation), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public IActionResult CreateCompensation([FromBody] CompensationDto compensation)
        {
            _logger.LogInformation($"Received compensation create request");

            try
            {
                //validate compensation
                if (compensation == null || string.IsNullOrWhiteSpace(compensation.EmployeeId))
                {
                    _logger.LogWarning("Invalid compensation data.");
                    return BadRequest("Compensation data was invalid.");
                }

                //validate salary value
                if (compensation.Salary < 0)
                {
                    _logger.LogWarning("Invalid salary value.");
                    return BadRequest("Salary must be 0 or greater.");
                }

                //create compensation
                var createdCompensation = _compensationService.Create(compensation);

                _logger.LogInformation($"Compensation successfully created for Employee Id {createdCompensation.EmployeeId}.");

                //return created compensation
                return CreatedAtRoute("getCompensationByEmployeeId", new { employeeId = createdCompensation.EmployeeId }, createdCompensation);
            }
            catch (ArgumentException ex) //handle employee not being found
            {
                _logger.LogError(ex, "Exception occurred while creating Compensation.");
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex) //handle case where compensation already exists for employee
            {
                _logger.LogError(ex, "Exception occurred while creating Compensation.");
                return Conflict(ex.Message);
            }
            catch (Exception ex) //handle any other exceptions
            {
                _logger.LogError(ex, "Exception occurred while creating Compensation.");
                return StatusCode(500, ex.Message);
            }

        }

        #endregion
    }
}
