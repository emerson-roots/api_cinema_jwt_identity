using SQLite;

namespace A3_API_Project.Models.Cinema
{
    public class Cidade
    {

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        //[NotNull]
        //[ForeignKey(typeof(Regiao))]
        //public int RegiaoId { get; set; }

        public string Estado { get; set; }

        public string Descricao { get; set; }

    }
}
