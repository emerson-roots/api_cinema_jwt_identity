using SQLite;

namespace A3_API_Project.Models.Cinema
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Username { get; set; }

        public string Senha { get; set; }
    }
}
