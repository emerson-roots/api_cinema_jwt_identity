using SQLite;

namespace A3_API_Project.Models.IDP
{
    public class ApplicationRole
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }
}
