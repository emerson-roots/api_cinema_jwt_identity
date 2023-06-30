using SQLite;

namespace A3_API_Project.Models.Cinema
{
    // tabela de amarração muitos para muitos
    [Table("Amr_FilmeCinema")]
    public class AmarracaoFilmeCinema
    {
        public int ID { get; set; }
        public int FilmeId { get; set; }
        public int CinemaId { get; set; }
    }
}
