using A3_API_Project.Models.Cinema;
using A3_API_Project.Models.Cinema.DTO;
using A3_API_Project.Repository.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace A3_API_Project.Repository
{
    public class CinemaRepository
    {

        private readonly DbSession _conn;

        public CinemaRepository(DbSession conn)
        {
            _conn = conn;
        }

        public async Task<int> InsertCinemaAsync(CinemaDTO cinemaDTO)
        {

            // valida amr cidade regiao
            var cidadeRegiao = await _conn.Database.Connection.Table<AmarracaoCidadeRegiao>().Where(i => i.ID == cinemaDTO.AmrCidadeRegiaoId).FirstOrDefaultAsync();

            if (cidadeRegiao is null)
                throw new Exception("Cidade/Região não encontrada;");

            // verifica se existe um cinema com mesmo nome e cidade/regiao cadastrado
            var cinemaMesmoNome = await _conn.Database.Connection.Table<Cinema>().Where(i => i.NomeCinema == cinemaDTO.NomeCinema && i.AmrCidadeRegiaoId == cinemaDTO.AmrCidadeRegiaoId ).FirstOrDefaultAsync();
            if (cinemaMesmoNome != null)
                throw new Exception("Já existe um cinema cadastrado com este mesmo nome.");

            var resultRetorno = 0;
            await _conn.Database.Connection.RunInTransactionAsync((SQLiteConnection transaction) =>
            {
                string sql = @"INSERT INTO Cinema (AmrCidadeRegiaoId,NomeCinema, Endereco, Longitude, Latitude, NotaAvaliacao) VALUES( ? ,? ,?, ?, ? , ?);";
                string[] parametros = new string[6] { 
                    cinemaDTO.AmrCidadeRegiaoId.ToString(),
                    cinemaDTO.NomeCinema,
                    cinemaDTO.Endereco,
                    cinemaDTO.Longitude.ToString(),
                    cinemaDTO.Latitude.ToString(),
                    cinemaDTO.NotaAvaliacao.ToString()
                };

                int resultRetorno = transaction.Execute(sql, parametros);

                // cadastra foto
                if (resultRetorno > 0 && !string.IsNullOrEmpty(cinemaDTO.FotoCinemaBase64))
                {
                    //FotoCinema foto = new FotoCinema()
                    //{
                    //    CinemaId = transaction.LastInsertRowId,
                    //    Foto = cinemaDTO.FotoCinemaBase64,
                    //};

                    //transaction.Insert(foto);
                }

            });

            return resultRetorno;
        }

        public async Task<List<Cinema>> GetCinemasPorRegiao(int regiaoId)
        {
            // recupera amarracao por REGIAO
            List<AmarracaoCidadeRegiao> amrCidadeRegiao = await _conn.Database.Connection.Table<AmarracaoCidadeRegiao>().Where(i => i.RegiaoId == regiaoId).ToListAsync();
            List<Cinema> cinemas = await GetCinemasPorAmarracaoCidadeRegiao(amrCidadeRegiao);
            return cinemas;
        }

        public async Task<List<Cinema>> GetCinemasPorCidade(int cidadeId)
        {
            // recupera amarracao por CIDADE
            List<AmarracaoCidadeRegiao> listAmarracaoCidadeRegiao = await _conn.Database.Connection.Table<AmarracaoCidadeRegiao>().Where(i => i.CidadeId == cidadeId).ToListAsync();
            List<Cinema> cinemas = await GetCinemasPorAmarracaoCidadeRegiao(listAmarracaoCidadeRegiao);
            return cinemas;
        }

        private async Task<List<Cinema>> GetCinemasPorAmarracaoCidadeRegiao(List<AmarracaoCidadeRegiao> amrCidadeRegiao)
        {
            // instancia a lista do retorno do método
            List<Cinema> cinemas = new List<Cinema>();

            // percorre as amarrações adicionando os cinemas de uma determinada amarracao
            foreach (var cidadeRegiao in amrCidadeRegiao)
            {
                var list = await _conn.Database.Connection.Table<Cinema>().Where(i => i.AmrCidadeRegiaoId == cidadeRegiao.ID).ToListAsync();
                if (list != null && list.Count() > 0)
                {
                    // preenche os objetos
                    foreach (var cin in list)
                    {
                        cin.Filmes = await GetFilmesPorCinema(cin.ID);
                        cin.Cidade = await _conn.Database.Connection.Table<Cidade>().Where(i => i.ID == cidadeRegiao.CidadeId).FirstOrDefaultAsync();
                        cin.FotosCinema = await _conn.Database.Connection.Table<FotoCinema>().Where(i => i.CinemaId == cin.ID).ToListAsync();
                    }
                }

                cinemas.AddRange(list);
            }

            return cinemas != null && cinemas.Count > 0 ? cinemas : null;
        }

        private async Task<List<Filme>> GetFilmesPorCinema(int idCinema)
        {
            List<AmarracaoFilmeCinema> list = await _conn.Database.Connection.Table<AmarracaoFilmeCinema>().Where(i => i.CinemaId == idCinema).ToListAsync();
            List<Filme> listFilmes = new List<Filme>();
            foreach (var filmeCinema in list)
            {

                var filme = await _conn.Database.Connection.Table<Filme>().Where(i => i.ID == filmeCinema.FilmeId).FirstOrDefaultAsync();
                var sessao = await _conn.Database.Connection.Table<Sessao>().Where(i => i.FilmeCinemaId == filmeCinema.ID).ToListAsync();
                if (sessao != null && sessao.Count() > 0)
                {
                    filme.Sessoes = new List<Sessao>();
                    filme.Sessoes.AddRange(sessao);
                }
                listFilmes.Add(filme);
            }

            return listFilmes != null && listFilmes.Count > 0 ? listFilmes : null;
        }
    }
}
