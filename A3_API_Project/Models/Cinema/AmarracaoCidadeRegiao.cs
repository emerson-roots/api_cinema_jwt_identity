using SQLite;
using SQLiteNetExtensions.Attributes;

namespace A3_API_Project.Models.Cinema
{
    [Table("Amr_CidadeRegiao")]
    public class AmarracaoCidadeRegiao
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        [NotNull]
        [ForeignKey(typeof(Cidade))]
        public int CidadeId { get; set; }

        [NotNull]
        [ForeignKey(typeof(Regiao))]
        public int RegiaoId { get; set; }
    }
}
