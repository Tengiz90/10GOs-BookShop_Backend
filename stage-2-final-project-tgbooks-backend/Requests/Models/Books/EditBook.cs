using stage_2_final_project_tgbooks_backend.Enums;

namespace stage_2_final_project_tgbooks_backend.Requests.Models.Books
{
    public class EditBook
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> AuthorNames { get; set; } = new List<string>();
        public Language Language { get; set; }
        public int Quantity { get; set; }
        public IFormFile? Image { get; set; }
        public string ImageUrlBefore {  get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
