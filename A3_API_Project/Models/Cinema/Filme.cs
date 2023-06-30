using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace A3_API_Project.Models.Cinema
{
    public class Filme
    {
        public int ID { get; set; }

        //[NotNull]
        //[ForeignKey(typeof(Cinema))]
        //public int CinemaId { get; set; }

        public string NomeFilme { get; set; }
        public string Sinopse { get; set; }
        public string Diretor { get; set; }
        public string Idioma { get; set; }
        public int Duracao { get; set; }
        public int NotaAvaliacao { get; set; }
        public string FotoCapaFilme { get; set; }
        public int FaixaEtaria { get; set; }
        public string Categoria { get; set; }

        [OneToMany]
        public List<Sessao> Sessoes { get; set; }

    }
}
