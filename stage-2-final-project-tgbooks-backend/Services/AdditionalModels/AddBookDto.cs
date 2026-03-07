using stage_2_final_project_tgbooks_backend.Enums;

namespace stage_2_final_project_tgbooks_backend.Services.AdditionalModels
{
    public class AddBookDto
    {
        public string Title { get; set; }
        public List<string> AuthorNames { get; set; }
        public Language Language { get; set; }
        public int Quantity { get; set; }
        public ICollection<int> CategoryIds { get; set; }
        public string ImageUrl { get; set; }
    }
}
