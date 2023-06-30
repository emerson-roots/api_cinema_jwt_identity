using SQLite;
using SQLiteNetExtensions.Attributes;

namespace A3_API_Project.Models.Cinema
{
    [Table("Amr_PerfilUsuario")]
    public class AmarracaoPerfilUsuario
    {
        [PrimaryKey]
        public int ID { get; set; }

        [NotNull]
        [ForeignKey(typeof(Usuario))]
        public int UsuarioId { get; set; }

        [NotNull]
        [ForeignKey(typeof(Perfil))]
        public int PerfilId { get; set; }
    }
}
