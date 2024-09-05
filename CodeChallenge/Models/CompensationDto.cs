using System;

namespace CodeChallenge.Models
{
    public class CompensationDto
    {
        public string EmployeeId { get; set; }
        public float Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
