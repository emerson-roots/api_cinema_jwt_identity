using A3_API_Project.Models.IDP;
using System.Threading.Tasks;

namespace A3_API_Project.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task<int> CreateAsync(ApplicationRole role);
        Task<int> DeleteAsync(ApplicationRole role);
        Task<int> UpdateAsync(ApplicationRole role);
        Task<ApplicationRole> FindByIdAsync(int roleId);
        Task<ApplicationRole> FindByNameAsync(string normalizedRoleName);
    }
}
