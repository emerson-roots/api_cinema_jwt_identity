using A3_API_Project.Models.IDP;
using A3_API_Project.Repository.Interfaces;
using A3_API_Project.Repository.Utils;
using System;
using System.Threading.Tasks;

namespace A3_API_Project.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DbSession _conn;

        public RoleRepository(DbSession conn) => _conn = conn;

        public async Task<int> CreateAsync(ApplicationRole role)
        {
            return await _conn.Database.Connection.InsertAsync(role);
        }

        public async Task<int> DeleteAsync(ApplicationRole role)
        {
            var obj = await FindByIdAsync(role.Id);
            if (obj is null) throw new Exception("Role/permissão não encontrada. Não é possível deletar.");

            return await _conn.Database.Connection.DeleteAsync<ApplicationRole>(role.Id);
        }

        public async Task<ApplicationRole> FindByIdAsync(int roleId)
        {
            return await _conn.Database.Connection.Table<ApplicationRole>()
                .Where(x => x.Id == roleId).FirstOrDefaultAsync();
        }

        public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName)
        {
            return await _conn.Database.Connection.Table<ApplicationRole>()
                .Where(x => x.NormalizedName.Equals(normalizedRoleName)).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateAsync(ApplicationRole role)
        {
            var obj = await FindByIdAsync(role.Id);
            if (obj is null) throw new Exception("Role não encontrada. Não é possível atualizar.");

            return await _conn.Database.Connection.UpdateAsync(role);
        }
    }
}
