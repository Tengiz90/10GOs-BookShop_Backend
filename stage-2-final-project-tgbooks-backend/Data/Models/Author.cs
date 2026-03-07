using stage_2_final_project_tgbooks_backend.Core;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class Author : BaseEntity
    {
        public string Name { get; set; } = null!;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
