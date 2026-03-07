using stage_2_final_project_tgbooks_backend.Core;
using stage_2_final_project_tgbooks_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace stage_2_final_project_tgbooks_backend.Data.Models
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public Language Language { get; set; }
        public int Quantity { get; set; }

        public string ImageURL { get; set; }

        [Timestamp]  // EF Core will use this for concurrency checks
        public byte[] RowVersion { get; set; } = null!;

        // Many-to-many
        public ICollection<Category> Categories { get; set; } = new List<Category>();


    }
}
