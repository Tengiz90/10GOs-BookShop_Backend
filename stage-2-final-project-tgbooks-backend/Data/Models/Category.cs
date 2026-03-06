using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Type { get; set; }

        // Many-to-many
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
