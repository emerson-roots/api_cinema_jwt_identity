using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace A3_API_Project.Models.Cinema
{
    public class Regiao
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Descricao { get; set; }

        [OneToMany]
        public List<Cidade> Cidades { get; set; }

    }
}
