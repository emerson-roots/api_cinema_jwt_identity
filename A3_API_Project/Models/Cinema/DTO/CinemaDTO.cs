using System.ComponentModel.DataAnnotations;

namespace A3_API_Project.Models.Cinema.DTO
{
    public class CinemaDTO
    {
        public int AmrCidadeRegiaoId { get; set; }
        public string NomeCinema { get; set; }
        public string Endereco { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double NotaAvaliacao { get; set; }
        public string FotoCinemaBase64{ get; set; }
    }
}
