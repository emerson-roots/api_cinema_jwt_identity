using SQLite;
using SQLiteNetExtensions.Attributes;

namespace A3_API_Project.Models.Cinema
{
    public class FotoCinema
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        [ForeignKey(typeof(Cinema))]
        public int CinemaId { get; set; }

        [NotNull]
        public string Foto { get; set; }
    }
}
