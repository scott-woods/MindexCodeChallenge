using CodeChallenge.Models;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    /// <summary>
    /// Compensation repository interface
    /// </summary>
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        Compensation GetByEmployeeId(string employeeId);
        Task SaveAsync();
    }
}
