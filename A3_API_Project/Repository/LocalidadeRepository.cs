using A3_API_Project.Models.Cinema;
using A3_API_Project.Repository.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3_API_Project.Repository
{
    public class LocalidadeRepository
    {

        private readonly DbSession _conn;

        public LocalidadeRepository(DbSession conn)
        {
            _conn = conn;
        }
        public async Task<List<Regiao>> GetAllRegioes()
        {

            List<Regiao> list = await _conn.Database.Connection.Table<Regiao>().ToListAsync();
            foreach (var regiao in list)
            {
                List<AmarracaoCidadeRegiao> amarracao = await _conn.Database.Connection.Table<AmarracaoCidadeRegiao>().Where(i => i.RegiaoId == regiao.ID).ToListAsync();
                if (amarracao != null && amarracao.Count > 0)
                    regiao.Cidades = new List<Cidade>();

                foreach (var itemAmarracao in amarracao)
                    regiao.Cidades.Add(await _conn.Database.Connection.Table<Cidade>().Where(i => i.ID == itemAmarracao.CidadeId).FirstOrDefaultAsync());
            }
            return list;
        }

        public async Task<List<Cidade>> GetAllCidades()
        {

            List<Cidade> list = await _conn.Database.Connection.Table<Cidade>().ToListAsync();
            return list;
        }

        public async Task<List<Cidade>> GetCidadesPorRegiao(int regiaoId)
        {

            List<AmarracaoCidadeRegiao> amarracao = await _conn.Database.Connection.Table<AmarracaoCidadeRegiao>().Where(i => i.RegiaoId == regiaoId).ToListAsync();

            List<Cidade> list = new List<Cidade>();
            foreach (var itemAmarracao in amarracao)
                list.Add(await _conn.Database.Connection.Table<Cidade>().Where(i => i.ID == itemAmarracao.CidadeId).FirstOrDefaultAsync());

            return list;
        }

    }
}
