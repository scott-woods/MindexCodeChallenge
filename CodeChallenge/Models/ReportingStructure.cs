namespace CodeChallenge.Models
{
    /// <summary>
    /// Manage an employee and their direct reports
    /// </summary>
    public class ReportingStructure
    {
        /// <summary>
        /// Employee object
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Total number of reports for the employee
        /// </summary>
        public int NumberOfReports { get; set; }
    }
}
