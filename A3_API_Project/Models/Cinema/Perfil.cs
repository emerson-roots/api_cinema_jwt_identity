using SQLite;

namespace A3_API_Project.Models.Cinema
{
    public class Perfil
    {
        [PrimaryKey]
        public int ID { get; set; }

        public string Descricao { get; set; }
    }
}
