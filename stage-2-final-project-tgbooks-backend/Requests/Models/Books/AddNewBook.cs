using stage_2_final_project_tgbooks_backend.Enums;
using System.ComponentModel.DataAnnotations;

namespace stage_2_final_project_tgbooks_backend.Requests.Models.Books
{
    public class AddNewBook
    {
        public string Title { get; set; }
        public List<string> AuthorNames { get; set; } = new List<string>();
        public Language Language { get; set; }
        public int Quantity { get; set; }
        public decimal OriginalPrice { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public ICollection<int> CategoryIds { get; set; } = new List<int>();

    }
}
