using CodeChallenge.Models;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation Create(CompensationDto compensationDto);
        Compensation GetByEmployeeId(string employeeId);
    }
}
