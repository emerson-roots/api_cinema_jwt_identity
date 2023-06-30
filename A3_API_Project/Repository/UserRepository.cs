using A3_API_Project.Models.IDP;
using A3_API_Project.Repository.Interfaces;
using A3_API_Project.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3_API_Project.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DbSession _conn;

        public UserRepository(DbSession conn) => _conn = conn;

        public async Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            int? roleId = await FindRoleIdByName(roleName);
            if (!roleId.HasValue)
                throw new Exception("Role/Permissão não cadastrado. Não é possível adicionar permissão a este usuário.");

            string[] parametrosRoleAdd = new string[2] { user.Id.ToString(), roleId.ToString() };
            string queryRoleAdd = @"INSERT INTO ApplicationUserRole(UserId, RoleId) 
                                        SELECT ?, ? 
                                    WHERE NOT EXISTS
                                        (SELECT 1 FROM ApplicationUserRole 
                                    WHERE UserId = ? AND RoleId = ?);";

            await _conn.Database.Connection.ExecuteAsync(queryRoleAdd, parametrosRoleAdd);
        }

        public async Task<int> CreateAsync(ApplicationUser user)
        {
            return await _conn.Database.Connection.InsertAsync(user);
        }

        public async Task<int> DeleteAsync(ApplicationUser user)
        {
            var obj = await FindByIdAsync(user.Id);
            if (obj is null) throw new Exception("Usuário não encontrado. Não é possível deletar.");

            return await _conn.Database.Connection.DeleteAsync<ApplicationUser>(user.Id);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail)
        {
            return await _conn.Database.Connection.Table<ApplicationUser>()
                .Where(x => x.NormalizedEmail.Equals(normalizedEmail)).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> FindByIdAsync(int userId)
        {
            return await _conn.Database.Connection.Table<ApplicationUser>()
                .Where(x => x.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName)
        {
            return await _conn.Database.Connection.Table<ApplicationUser>()
                .Where(x => x.NormalizedUserName.Equals(normalizedUserName)).FirstOrDefaultAsync();
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            string query = @"SELECT r.Name FROM ApplicationRole r INNER JOIN ApplicationUserRole ur ON ur.RoleId = r.Id WHERE ur.UserId = ?;";
            List<ApplicationRole> result = await _conn.Database.Connection.QueryAsync<ApplicationRole>(query, user.Id);

            IList<string> listReturn = new List<string>();
            result.ForEach(x => listReturn.Add(x.Name));

            return listReturn;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName)
        {

            string query = @"SELECT u.* FROM ApplicationUser u 
                                INNER JOIN ApplicationUserRole ur ON ur.UserId = u.Id 
                                INNER JOIN ApplicationRole r ON r.Id = ur.RoleId 
                                WHERE r.NormalizedName = ?;";

            List<ApplicationUser> result = await _conn.Database.Connection.QueryAsync<ApplicationUser>(query, roleName.ToUpper());
            return result;
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            int? roleId = await FindRoleIdByName(roleName);
            if (roleId == default(int)) return false;

            string[] parametros = { user.Id.ToString(), roleId.ToString() };
            string queryMatchingRoles = @"SELECT COUNT(*) FROM ApplicationUserRole WHERE UserId = ? AND RoleId = ?;";
            var matchRoles = await _conn.Database.Connection.ExecuteScalarAsync<int>(queryMatchingRoles, parametros);

            return matchRoles > 0;
        }

        private async Task<int?> FindRoleIdByName(string roleName)
        {
            string query = @"SELECT Id FROM ApplicationRole WHERE NormalizedName = ?";
            var roleId = await _conn.Database.Connection.ExecuteScalarAsync<int?>(query, roleName.ToUpper());
            return roleId;
        }

        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            int? roleId = await FindRoleIdByName(roleName);
            if (roleId.HasValue)
            {
                string[] parametros = { user.Id.ToString(), roleId.ToString() };
                await _conn.Database.Connection.ExecuteAsync($"DELETE FROM ApplicationUserRole WHERE UserId = ? AND RoleId = ?", parametros);
            }
        }

        public async Task<int> UpdateAsync(ApplicationUser user)
        {
            var obj = await FindByIdAsync(user.Id);
            if (obj is null) throw new Exception("Usuário não encontrado. Não é possível atualizar.");

            return await _conn.Database.Connection.UpdateAsync(user);
        }
    }
}
