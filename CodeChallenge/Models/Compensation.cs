using System;

namespace CodeChallenge.Models
{
    /// <summary>
    /// Compensation model
    /// </summary>
    public class Compensation
    {
        /// <summary>
        /// Id
        /// </summary>
        public String CompensationId { get; set; }

        /// <summary>
        /// Employee id
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Salary
        /// </summary>
        public float Salary { get; set; }

        /// <summary>
        /// Effective date
        /// </summary>
        public DateTime EffectiveDate { get; set; }


        //NAVIGATION PROPERTIES

        /// <summary>
        /// Employee this compensation is for
        /// </summary>
        public virtual Employee Employee { get; set; }
    }
}
