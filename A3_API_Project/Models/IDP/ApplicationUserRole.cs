using SQLite;

namespace A3_API_Project.Models.IDP
{
    // classe para gerenciar as permissões associadas a um determinado usuário
    // foi criada apenas para que o ORM do sqlite pudesse gerenciar e criar a tabela automaticamente
    // mas não era necessário, pode ser criada a tabela manualmente...
    public class ApplicationUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
