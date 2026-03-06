using stage_2_final_project_tgbooks_backend.Enums;

namespace stage_2_final_project_tgbooks_backend.Requests.Models.Books
{
    public class AddNewBook
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public Language Language { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
        public ICollection<int> CategoryIds { get; set; } = new List<int>();

    }
}
