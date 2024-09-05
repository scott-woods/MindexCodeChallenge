using System.Collections.Generic;
using System;

namespace CodeChallenge.Models
{
    public class EmployeeDto
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Position { get; set; }
        public String Department { get; set; }
        public List<string> DirectReportIds { get; set; }
    }
}
