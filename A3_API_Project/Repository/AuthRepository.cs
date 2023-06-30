using A3_API_Project.Models;
using A3_API_Project.Models.Cinema;
using A3_API_Project.Repository.Utils;
using System;
using System.Threading.Tasks;

namespace A3_API_Project.Repository
{
    public class AuthRepository
    {

        private readonly DbSession _conn;

        public AuthRepository(DbSession conn)
        {
            _conn = conn;
        }

        //public async Task<bool> CadastrarUsuario(User user)
        //{
        //    Usuario obj = new Usuario() { Username = user.Usuario, Senha = user.Senha };

        //    if (string.IsNullOrEmpty(obj.Username)
        //        || string.IsNullOrWhiteSpace(obj.Username)
        //        || obj.Username.Length < 5)
        //    {
        //        throw new Exception("Usuario inválido. Deve conter no mínimo 5 caracteres e sem espaços em branco.");
        //    }
        //    else if (string.IsNullOrEmpty(obj.Senha)
        //      || string.IsNullOrWhiteSpace(obj.Senha)
        //      || obj.Senha.Length < 5)
        //    {
        //        throw new Exception("Senha inválida. Deve conter no mínimo 5 caracteres e sem espaços em branco.");
        //    }


        //    var possuiCadastro = await _conn.Database.Connection.Table<Usuario>().Where(i => i.Username == obj.Username).FirstOrDefaultAsync();
        //    if (possuiCadastro != null)
        //        throw new Exception("Este e-mail já possui cadastro.");

        //    int result = await _conn.Database.Connection.InsertAsync(obj, typeof(Usuario));
        //    return result > 0 ? true : false;
        //}

        //public async Task<bool> AutenticarUsuario(User user)
        //{
        //    Usuario obj = new Usuario() { Username = user.Usuario, Senha = user.Senha };

        //    var usuarioEncontrado = await _conn.Database.Connection.Table<Usuario>().Where(i => i.Username == obj.Username && i.Senha == obj.Senha).FirstOrDefaultAsync();
        //    return usuarioEncontrado != null ? true : false;
        //}

        public async Task<bool> IsPossuiCadastro(string email)
        {
            Usuario obj = new Usuario() { Username = email };

            var usuarioEncontrado = await _conn.Database.Connection.Table<Usuario>().Where(i => i.Username == obj.Username).FirstOrDefaultAsync();
            return usuarioEncontrado != null ? true : false;
        }
        

    }
}
