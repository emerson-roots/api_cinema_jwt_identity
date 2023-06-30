using SQLite;
using SQLiteNetExtensions.Attributes;
using System;

namespace A3_API_Project.Models.Cinema
{
    public class Sessao
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        [ForeignKey(typeof(AmarracaoFilmeCinema))]
        public int FilmeCinemaId { get; set; }


        public DateTime DataHora { get; set; }

    }
}
