using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace A3_API_Project.Models.Cinema
{

    public class Cinema
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        //[NotNull]
        //[ForeignKey(typeof(Cidade))]
        //public int CidadeId { get; set; }
        public int AmrCidadeRegiaoId { get; set; }

        public string NomeCinema { get; set; }
        public string Endereco { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double NotaAvaliacao { get; set; }

        [OneToMany]
        public List<FotoCinema> FotosCinema { get; set; }

        [ManyToOne]
        public Cidade Cidade { get; set; }

        [OneToMany]
        public List<Filme> Filmes { get; set; }

    }

    //public class Regiao
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int ID { get; set; }

    //    public string Descricao { get; set; }

    //    [OneToMany]
    //    public List<Cidade> Cidades { get; set; }

    //}

    //public class Cidade
    //{

    //    [PrimaryKey, AutoIncrement]
    //    public int ID { get; set; }

    //    //[NotNull]
    //    //[ForeignKey(typeof(Regiao))]
    //    //public int RegiaoId { get; set; }

    //    public string Estado { get; set; }

    //    public string Descricao { get; set; }

    //}

    //[Table("Amr_CidadeRegiao")]
    //public class AmarracaoCidadeRegiao
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int ID { get; set; }

    //    [NotNull]
    //    [ForeignKey(typeof(Cidade))]
    //    public int CidadeId { get; set; }

    //    [NotNull]
    //    [ForeignKey(typeof(Regiao))]
    //    public int RegiaoId { get; set; }
    //}





    //public class FotoCinema
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int ID { get; set; }

    //    [NotNull]
    //    [ForeignKey(typeof(Cinema))]
    //    public int CinemaId { get; set; }

    //    [NotNull]
    //    public string Foto { get; set; }
    //}

    //public class Filme
    //{
    //    public int ID { get; set; }

    //    //[NotNull]
    //    //[ForeignKey(typeof(Cinema))]
    //    //public int CinemaId { get; set; }

    //    public string NomeFilme { get; set; }
    //    public string Sinopse { get; set; }
    //    public string Diretor { get; set; }
    //    public string Idioma { get; set; }
    //    public int Duracao { get; set; }
    //    public int NotaAvaliacao { get; set; }
    //    public string FotoCapaFilme { get; set; }
    //    public int FaixaEtaria { get; set; }
    //    public string Categoria { get; set; }

    //    [OneToMany]
    //    public List<Sessao> Sessoes { get; set; }

    //}

    //// tabela de amarração muitos para muitos
    //[Table("Amr_FilmeCinema")]
    //public class AmarracaoFilmeCinema
    //{
    //    public int ID { get; set; }
    //    public int FilmeId { get; set; }
    //    public int CinemaId { get; set; }
    //}

    //public class Sessao
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int ID { get; set; }

    //    [NotNull]
    //    [ForeignKey(typeof(AmarracaoFilmeCinema))]
    //    public int FilmeCinemaId { get; set; }


    //    public DateTime DataHora { get; set; }

    //}

    //public class Usuario
    //{
    //    [PrimaryKey, AutoIncrement]
    //    public int ID { get; set; }

    //    public string Username { get; set; }

    //    public string Senha { get; set; }
    //}

    //public class Perfil
    //{
    //    [PrimaryKey]
    //    public int ID { get; set; }

    //    public string Descricao { get; set; }
    //}

    //[Table("Amr_PerfilUsuario")]
    //public class AmarracaoPerfilUsuario
    //{
    //    [PrimaryKey]
    //    public int ID { get; set; }

    //    [NotNull]
    //    [ForeignKey(typeof(Usuario))]
    //    public int UsuarioId { get; set; }

    //    [NotNull]
    //    [ForeignKey(typeof(Perfil))]
    //    public int PerfilId { get; set; }
    //}
}
